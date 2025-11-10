namespace BankMore.Tarifas.Models;

/// <summary>
/// Evento de transferência realizada (consumido do Kafka)
/// </summary>
public class TransferenciaRealizadaEvent
{
    public string IdTransferencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public string IdContaCorrenteDestino { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string DataMovimento { get; set; } = string.Empty;
    public decimal ValorTarifa { get; set; }
}
