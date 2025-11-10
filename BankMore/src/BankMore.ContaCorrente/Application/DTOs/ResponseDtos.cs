namespace BankMore.ContaCorrente.Application.DTOs;

/// <summary>
/// DTO para resposta de cadastro de conta
/// </summary>
public class ContaCadastradaDto
{
    public int NumeroConta { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}

/// <summary>
/// DTO para resposta de login
/// </summary>
public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public int NumeroConta { get; set; }
    public int ExpiresInMinutes { get; set; }
}

/// <summary>
/// DTO para consulta de saldo
/// </summary>
public class SaldoDto
{
    public int NumeroConta { get; set; }
    public string NomeTitular { get; set; } = string.Empty;
    public string DataHoraConsulta { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}

/// <summary>
/// DTO para erros de validação
/// </summary>
public class ErroDto
{
    public string Tipo { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}
