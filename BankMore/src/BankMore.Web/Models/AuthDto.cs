namespace BankMore.Web.Models;

public class LoginRequest
{
    public string NumeroContaOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public int NumeroConta { get; set; }
    public int ExpiresInMinutes { get; set; }
    
    // Para manter compatibilidade com código existente
    public string Token => AccessToken;
    public DateTime Expiration => DateTime.UtcNow.AddMinutes(ExpiresInMinutes);
}

public class CadastrarContaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
