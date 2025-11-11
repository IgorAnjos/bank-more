using System.Net.Http.Headers;
using System.Net.Http.Json;
using BankMore.Web.Models;

namespace BankMore.Web.Services;

public class TransferenciaService
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;

    public TransferenciaService(HttpClient httpClient, TokenService tokenService)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
    }

    private void AddAuthorizationHeader()
    {
        var token = _tokenService.GetToken();
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }

    public async Task<TransferenciaDto?> RealizarTransferenciaAsync(RealizarTransferenciaRequest request)
    {
        AddAuthorizationHeader();
        var response = await _httpClient.PostAsJsonAsync("/api/v1/transferencias", request);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            
            // Tentar extrair mensagem de erro do JSON de resposta
            if (errorContent.Contains("INSUFFICIENT_BALANCE") || errorContent.Contains("Saldo insuficiente"))
            {
                throw new HttpRequestException("Saldo insuficiente. Verifique seu saldo e tente novamente.");
            }
            
            response.EnsureSuccessStatusCode(); // Lança exceção padrão
        }
        
        return await response.Content.ReadFromJsonAsync<TransferenciaDto>();
    }

    public async Task<TransferenciaDto?> ObterTransferenciaAsync(string id)
    {
        AddAuthorizationHeader();
        return await _httpClient.GetFromJsonAsync<TransferenciaDto>($"/api/v1/transferencias/{id}");
    }

    public async Task<PaginatedList<TransferenciaDto>?> ListarTransferenciasAsync(
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int page = 1,
        int pageSize = 20)
    {
        AddAuthorizationHeader();
        
        var query = $"/api/v1/transferencias?page={page}&pageSize={pageSize}";
        if (dataInicio.HasValue) query += $"&dataInicio={dataInicio:yyyy-MM-dd}";
        if (dataFim.HasValue) query += $"&dataFim={dataFim:yyyy-MM-dd}";

        return await _httpClient.GetFromJsonAsync<PaginatedList<TransferenciaDto>>(query);
    }
}
