namespace BankMore.ContaCorrente.Application.DTOs;

/// <summary>
/// Resposta padronizada para erros seguindo RFC 7807 (Problem Details)
/// </summary>
public class ApiProblemDetails
{
    /// <summary>
    /// URI que identifica o tipo do problema
    /// </summary>
    public string Type { get; set; } = "about:blank";

    /// <summary>
    /// Título legível do problema
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Código de status HTTP
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Explicação detalhada do problema
    /// </summary>
    public string Detail { get; set; } = string.Empty;

    /// <summary>
    /// URI que identifica a instância específica do problema
    /// </summary>
    public string Instance { get; set; } = string.Empty;

    /// <summary>
    /// Propriedades adicionais específicas do problema
    /// </summary>
    public Dictionary<string, object>? Extensions { get; set; }

    /// <summary>
    /// Timestamp do erro
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Código de erro interno (ex: INVALID_ACCOUNT)
    /// </summary>
    public string? ErrorCode { get; set; }
}
