namespace BankMore.ContaCorrente.Domain.Events;

/// <summary>
/// Evento publicado quando um movimento é realizado na conta
/// Topic: movimentos-realizados
/// </summary>
public class MovimentoRealizadoEvent
{
    public string IdMovimento { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
    public int NumeroConta { get; set; }
    public decimal Valor { get; set; }
    public string TipoMovimento { get; set; } = string.Empty; // Credito, Debito
    public DateTime DataMovimento { get; set; }
    public string? ChaveIdempotencia { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
