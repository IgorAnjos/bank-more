using System.Net.Http.Json;
using BankMore.Web.Models;
using Microsoft.AspNetCore.Components;

namespace BankMore.Web.Services;

public class AuthService
{
    private readonly HttpClient _httpClient;
    private readonly TokenService _tokenService;
    private readonly NavigationManager _navigationManager;

    public AuthService(HttpClient httpClient, TokenService tokenService, NavigationManager navigationManager)
    {
        _httpClient = httpClient;
        _tokenService = tokenService;
        _navigationManager = navigationManager;
    }

    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // Cria o payload com o campo correto que a API espera
        var payload = new
        {
            numeroOuCpf = request.NumeroContaOuCpf,
            senha = request.Senha
        };

        Console.WriteLine($"[AUTH SERVICE] Enviando payload: numeroOuCpf='{payload.numeroOuCpf}', senhaLength={payload.senha?.Length ?? 0}");
        
        var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/tokens", payload);
        
        Console.WriteLine($"[AUTH SERVICE] Status code: {response.StatusCode}");
        
        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse != null)
            {
                Console.WriteLine($"[AUTH SERVICE] Token recebido e salvo. ID Conta: {loginResponse.IdContaCorrente}");
                _tokenService.SetToken(loginResponse.AccessToken);
                _tokenService.SetIdConta(loginResponse.IdContaCorrente);
            }
            return loginResponse;
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[AUTH SERVICE] Erro: {errorContent}");
        }
        
        return null;
    }

    public async Task<HttpResponseMessage> ValidarSenhaAsync(string numeroOuCpf, string senha)
    {
        var payload = new
        {
            numeroOuCpf = numeroOuCpf,
            senha = senha
        };

        return await _httpClient.PostAsJsonAsync("/api/v1/auth/tokens", payload);
    }

    public async Task<ContaDto?> CadastrarContaAsync(CadastrarContaRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/contas", request);
        
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<ContaDto>();
        }
        
        return null;
    }

    public void Logout()
    {
        _tokenService.ClearToken();
        _navigationManager.NavigateTo("/login");
    }

    public bool IsAuthenticated()
    {
        return !string.IsNullOrEmpty(_tokenService.GetToken());
    }
}
