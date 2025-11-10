namespace BankMore.Transferencia.Domain.Events;

/// <summary>
/// Evento publicado quando uma compensação falha e precisa de processamento assíncrono
/// Topic: compensacoes-pendentes
/// </summary>
public class CompensacaoPendenteEvent
{
    public string IdTransferencia { get; set; } = string.Empty;
    public string IdContaOrigem { get; set; } = string.Empty;
    public decimal ValorEstorno { get; set; }
    public int TentativasRealizadas { get; set; }
    public string MotivoFalha { get; set; } = string.Empty;
    public string? ChaveIdempotencia { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
