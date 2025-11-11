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

    /// <summary>
    /// Obtém dados básicos de uma conta pelo número
    /// </summary>
    public async Task<ContaCorrenteDto?> ObterContaPorNumeroAsync(int numeroConta)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_apiUrl}/api/v1/contas/numero/{numeroConta}");
            
            if (!response.IsSuccessStatusCode)
                return null;

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonSerializer.Deserialize<JsonElement>(content);
            
            return new ContaCorrenteDto
            {
                Id = json.GetProperty("id").GetString() ?? string.Empty,
                Numero = json.GetProperty("numero").GetInt32(),
                Status = json.GetProperty("status").GetString() ?? string.Empty
            };
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> RealizarMovimentacaoAsync(
        string token, 
        string chaveIdempotencia, 
        int? numeroConta, 
        char tipoMovimento, 
        decimal valor)
    {
        try
        {
            // Se numeroConta for null, usa a conta do token (extrair do JWT)
            // Se tiver numeroConta, precisamos buscar o ID da conta primeiro
            string idConta = string.Empty;

            if (numeroConta.HasValue)
            {
                // Busca o ID da conta pelo número (endpoint de validação não retorna ID)
                // Vamos usar um endpoint que retorna dados da conta
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var getResponse = await _httpClient.GetAsync($"{_apiUrl}/api/v1/contas/numero/{numeroConta.Value}");
                
                if (!getResponse.IsSuccessStatusCode)
                {
                    throw new InvalidOperationException($"Conta {numeroConta.Value} não encontrada");
                }

                var contaData = await getResponse.Content.ReadAsStringAsync();
                var contaJson = JsonSerializer.Deserialize<JsonElement>(contaData);
                idConta = contaJson.GetProperty("id").GetString() ?? string.Empty;
            }
            else
            {
                // Extrai o ID da conta do token JWT (claim "sub")
                var tokenParts = token.Split('.');
                if (tokenParts.Length == 3)
                {
                    var payload = tokenParts[1];
                    // Adiciona padding se necessário
                    var paddingNeeded = (4 - (payload.Length % 4)) % 4;
                    payload += new string('=', paddingNeeded);
                    
                    var payloadBytes = Convert.FromBase64String(payload);
                    var payloadJson = Encoding.UTF8.GetString(payloadBytes);
                    var payloadData = JsonSerializer.Deserialize<JsonElement>(payloadJson);
                    idConta = payloadData.GetProperty("sub").GetString() ?? string.Empty;
                }
            }

            if (string.IsNullOrEmpty(idConta))
            {
                throw new InvalidOperationException("Não foi possível determinar o ID da conta");
            }

            var requestBody = new
            {
                ChaveIdempotencia = chaveIdempotencia,
                TipoMovimento = tipoMovimento,
                Valor = valor
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Adicionar token JWT no header (para logs/auditoria)
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            // Adicionar header de serviço interno para permitir movimentação em qualquer conta
            if (!_httpClient.DefaultRequestHeaders.Contains("X-Service-Key"))
            {
                _httpClient.DefaultRequestHeaders.Add("X-Service-Key", "BankMore-Internal-Service-Key-2024");
            }

            var response = await _httpClient.PostAsync($"{_apiUrl}/api/v1/contas/{idConta}/movimentos", httpContent);

            if (response.IsSuccessStatusCode)
                return true;

            // Se for 403 ou 401, token inválido
            if (response.StatusCode == System.Net.HttpStatusCode.Forbidden || 
                response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                throw new UnauthorizedAccessException("Token JWT inválido ou expirado");

            // Outros erros
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Erro ao realizar movimentação: {errorContent}");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Erro ao realizar movimentação: {ex.Message}", ex);
        }
    }
}
