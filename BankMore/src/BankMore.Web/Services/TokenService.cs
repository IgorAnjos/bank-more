using Microsoft.JSInterop;

namespace BankMore.Web.Services;

public class TokenService
{
    private readonly IJSRuntime _jsRuntime;
    private string? _token;
    private string? _idConta;

    public TokenService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task InitializeAsync()
    {
        try
        {
            _token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
            _idConta = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "idConta");
        }
        catch
        {
            _token = null;
            _idConta = null;
        }
    }

    public string? GetToken()
    {
        return _token;
    }

    public string? GetIdConta()
    {
        return _idConta;
    }

    public async void SetToken(string token)
    {
        _token = token;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
    }

    public async void SetIdConta(string idConta)
    {
        _idConta = idConta;
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "idConta", idConta);
    }

    public async void ClearToken()
    {
        _token = null;
        _idConta = null;
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "idConta");
    }
}
