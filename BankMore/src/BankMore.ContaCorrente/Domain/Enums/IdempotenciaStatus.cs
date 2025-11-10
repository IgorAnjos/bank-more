namespace BankMore.ContaCorrente.Domain.Enums;

/// <summary>
/// Status da requisição idempotente
/// </summary>
public enum IdempotenciaStatus
{
    /// <summary>
    /// Requisição em processamento
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Requisição processada com sucesso
    /// </summary>
    Success = 1,

    /// <summary>
    /// Requisição processada com falha
    /// </summary>
    Failed = 2
}
