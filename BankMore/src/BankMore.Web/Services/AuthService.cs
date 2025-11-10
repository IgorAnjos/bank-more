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
        var response = await _httpClient.PostAsJsonAsync("/api/v1/auth/login", request);
        
        if (response.IsSuccessStatusCode)
        {
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
            if (loginResponse != null)
            {
                _tokenService.SetToken(loginResponse.Token);
                _tokenService.SetIdConta(loginResponse.Conta?.Id ?? string.Empty);
            }
            return loginResponse;
        }
        
        return null;
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
