namespace BankMore.Transferencia.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa uma Transferência
/// </summary>
public class Transferencia
{
    public string Id { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty;
    public string IdContaCorrenteDestino { get; set; } = string.Empty;
    public string DataMovimento { get; set; } = string.Empty;
    public decimal Valor { get; set; }

    public Transferencia() { }

    public Transferencia(string idContaCorrenteOrigem, string idContaCorrenteDestino, decimal valor)
    {
        Id = Guid.CreateVersion7().ToString();
        IdContaCorrenteOrigem = idContaCorrenteOrigem ?? throw new ArgumentNullException(nameof(idContaCorrenteOrigem));
        IdContaCorrenteDestino = idContaCorrenteDestino ?? throw new ArgumentNullException(nameof(idContaCorrenteDestino));
        Valor = Math.Round(valor, 2);
        DataMovimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
