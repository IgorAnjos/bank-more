namespace BankMore.ContaCorrente.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa um Movimento (Crédito ou Débito)
/// </summary>
public class Movimento
{
    public string IdMovimento { get; private set; }
    public string IdContaCorrente { get; private set; }
    public string DataMovimento { get; private set; }
    public char TipoMovimento { get; private set; } // 'C' = Crédito, 'D' = Débito
    public decimal Valor { get; private set; }
    public decimal SaldoAnterior { get; private set; }
    public decimal SaldoAtualizado { get; private set; }

    // Navigation property
    public virtual ContaCorrente? ContaCorrente { get; private set; }

    // Construtor privado para EF Core
    private Movimento() 
    {
        IdMovimento = string.Empty;
        IdContaCorrente = string.Empty;
        DataMovimento = string.Empty;
    }

    // Construtor para criação de novo movimento
    public Movimento(string idContaCorrente, char tipoMovimento, decimal valor, decimal saldoAnterior = 0, decimal saldoAtualizado = 0)
    {
        if (string.IsNullOrWhiteSpace(idContaCorrente))
            throw new ArgumentNullException(nameof(idContaCorrente));

        if (tipoMovimento != 'C' && tipoMovimento != 'D')
            throw new ArgumentException("Tipo de movimento deve ser 'C' (Crédito) ou 'D' (Débito)", nameof(tipoMovimento));

        if (valor <= 0)
            throw new ArgumentException("Valor deve ser maior que zero", nameof(valor));

        IdMovimento = Guid.CreateVersion7().ToString();
        IdContaCorrente = idContaCorrente;
        TipoMovimento = tipoMovimento;
        Valor = Math.Round(valor, 2); // Garantir duas casas decimais
        SaldoAnterior = Math.Round(saldoAnterior, 2);
        SaldoAtualizado = Math.Round(saldoAtualizado, 2);
        DataMovimento = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
    }

    public bool IsCredito() => TipoMovimento == 'C';
    public bool IsDebito() => TipoMovimento == 'D';
}
