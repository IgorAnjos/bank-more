using System.Net.Http.Headers;
using System.Net.Http.Json;
using BankMore.Web.Models;

namespace BankMore.Web.Services;

public class ContaService
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;

    public ContaService(HttpClient httpClient, TokenService tokenService)
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

    public async Task<ContaDto?> ObterContaAsync(string id)
    {
        AddAuthorizationHeader();
        return await _httpClient.GetFromJsonAsync<ContaDto>($"/api/v1/contas/{id}");
    }

    public async Task<SaldoDto?> ConsultarSaldoAsync(string id)
    {
        AddAuthorizationHeader();
        return await _httpClient.GetFromJsonAsync<SaldoDto>($"/api/v1/contas/{id}/saldo");
    }

    public async Task<PaginatedList<MovimentoDto>?> ListarMovimentosAsync(
        string id,
        char? tipo = null,
        DateTime? dataInicio = null,
        DateTime? dataFim = null,
        int page = 1,
        int pageSize = 20)
    {
        AddAuthorizationHeader();
        
        var query = $"/api/v1/contas/{id}/movimentos?page={page}&pageSize={pageSize}";
        if (tipo.HasValue) query += $"&tipo={tipo}";
        if (dataInicio.HasValue) query += $"&dataInicio={dataInicio:yyyy-MM-dd}";
        if (dataFim.HasValue) query += $"&dataFim={dataFim:yyyy-MM-dd}";

        return await _httpClient.GetFromJsonAsync<PaginatedList<MovimentoDto>>(query);
    }

    public async Task<HttpResponseMessage> CriarMovimentoAsync(string id, MovimentacaoRequest request)
    {
        AddAuthorizationHeader();
        return await _httpClient.PostAsJsonAsync($"/api/v1/contas/{id}/movimentos", request);
    }
}
