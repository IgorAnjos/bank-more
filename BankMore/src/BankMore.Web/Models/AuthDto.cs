namespace BankMore.Web.Models;

public class LoginRequest
{
    public string NumeroContaOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public ContaDto? Conta { get; set; }
}

public class CadastrarContaRequest
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
