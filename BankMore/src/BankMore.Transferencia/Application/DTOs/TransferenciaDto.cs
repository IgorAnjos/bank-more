namespace BankMore.Transferencia.Application.DTOs;

/// <summary>
/// DTO para transferência com HATEOAS
/// </summary>
public class TransferenciaDto : IHateoasResource
{
    /// <summary>
    /// ID único da transferência
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID da conta corrente de origem
    /// </summary>
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta de origem
    /// </summary>
    public int? NumeroContaOrigem { get; set; }

    /// <summary>
    /// ID da conta corrente de destino
    /// </summary>
    public string IdContaCorrenteDestino { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta de destino
    /// </summary>
    public int NumeroContaDestino { get; set; }

    /// <summary>
    /// Valor da transferência
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Data/hora da transferência
    /// </summary>
    public string DataTransferencia { get; set; } = string.Empty;

    /// <summary>
    /// Status da transferência
    /// </summary>
    public string Status { get; set; } = "Concluída";

    /// <summary>
    /// Links HATEOAS
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
