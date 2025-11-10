namespace BankMore.Transferencia.Domain.Events;

/// <summary>
/// Evento publicado quando uma transferência é realizada com sucesso
/// Topic: transferencias-realizadas
/// </summary>
public class TransferenciaRealizadaEvent
{
    public string IdTransferencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public string IdContaCorrenteDestino { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string DataMovimento { get; set; } = string.Empty;
    public decimal ValorTarifa { get; set; } // Configurável no appsettings
    public string? ChaveIdempotencia { get; set; }
    public string EventId { get; set; } = Guid.NewGuid().ToString(); // Para idempotência do consumer
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
