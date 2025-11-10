namespace BankMore.Tarifas.Models;

/// <summary>
/// Entidade Tarifa para persistência
/// </summary>
public class Tarifa
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string IdContaCorrente { get; set; } = string.Empty;
    public string IdTransferencia { get; set; } = string.Empty;
    public decimal Valor { get; set; }
    public string DataMovimento { get; set; } = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");

    public Tarifa() { }

    public Tarifa(string idContaCorrente, string idTransferencia, decimal valor)
    {
        IdContaCorrente = idContaCorrente;
        IdTransferencia = idTransferencia;
        Valor = Math.Round(valor, 2);
    }
}
