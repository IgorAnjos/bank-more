namespace BankMore.ContaCorrente.Domain.Events;

/// <summary>
/// Evento publicado quando uma nova conta é criada
/// Topic: contas-criadas
/// </summary>
public class ContaCriadaEvent
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public int NumeroConta { get; set; }
    public DateTime DataCriacao { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString(); // Para idempotência
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
