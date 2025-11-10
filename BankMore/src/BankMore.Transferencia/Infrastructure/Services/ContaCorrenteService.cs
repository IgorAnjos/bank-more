using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Services;

/// <summary>
/// Cliente HTTP para comunicação com a API Conta Corrente
/// </summary>
public class ContaCorrenteService : IContaCorrenteService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl;

    public ContaCorrenteService(HttpClient httpClient, string apiUrl)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _apiUrl = apiUrl ?? throw new ArgumentNullException(nameof(apiUrl));
    }

    /// <summary>
    /// Valida se uma conta existe e está ativa através do número da conta (dado não sensível)
    /// Faz uma chamada HEAD ou GET para endpoint público que retorna apenas status
    /// </summary>
    public async Task<bool> ValidarContaExistenteAsync(int numeroConta)
    {
        try
        {
            // Endpoint público que valida existência de conta por número
            // Não expõe dados sensíveis (CPF, saldo, etc)
            var response = await _httpClient.GetAsync($"{_apiUrl}/api/v1/contas/validar/{numeroConta}");

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RealizarMovimentacaoAsync(
        string token, 
        string chaveIdempotencia, 
        int? numeroConta, 
        char tipoMovimento, 
        decimal valor)
    {
        var requestBody = new
        {
            ChaveIdempotencia = chaveIdempotencia,
            NumeroConta = numeroConta,
            TipoMovimento = tipoMovimento,
            Valor = valor
        };

        var jsonContent = JsonSerializer.Serialize(requestBody);
        var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Adicionar token JWT no header
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _httpClient.PostAsync($"{_apiUrl}/api/movimentacao", httpContent);

        if (response.IsSuccessStatusCode)
            return true;

        // Se for 403, token inválido
        if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            throw new UnauthorizedAccessException("Token JWT inválido ou expirado");

        // Outros erros
        var errorContent = await response.Content.ReadAsStringAsync();
        throw new InvalidOperationException($"Erro ao realizar movimentação: {errorContent}");
    }
}
