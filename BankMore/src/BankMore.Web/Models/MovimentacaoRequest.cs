namespace BankMore.Web.Models;

public class MovimentacaoRequest
{
    public string ChaveIdempotencia { get; set; } = Guid.CreateVersion7().ToString();
    public int? NumeroConta { get; set; }
    public char TipoMovimento { get; set; }
    public decimal Valor { get; set; }
}
