namespace BankMore.Web.Models;

public class TransferenciaDto
{
    public string Id { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public string IdContaCorrenteDestino { get; set; } = string.Empty;
    public int NumeroContaOrigem { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public decimal TarifaAplicada { get; set; }
    public DateTime DataTransferencia { get; set; }
    public string Status { get; set; } = string.Empty;
    public List<Link>? Links { get; set; }
}

public class RealizarTransferenciaRequest
{
    public string? ChaveIdempotencia { get; set; }
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
}
