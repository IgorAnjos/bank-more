namespace BankMore.ContaCorrente.Application.DTOs;

/// <summary>
/// DTO para movimento com HATEOAS
/// </summary>
public class MovimentoDto : IHateoasResource
{
    /// <summary>
    /// ID único do movimento
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// ID da conta corrente
    /// </summary>
    public string IdContaCorrente { get; set; } = string.Empty;

    /// <summary>
    /// Tipo de movimento (C = Crédito, D = Débito)
    /// </summary>
    public char TipoMovimento { get; set; }

    /// <summary>
    /// Valor do movimento
    /// </summary>
    public decimal Valor { get; set; }

    /// <summary>
    /// Saldo antes do movimento
    /// </summary>
    public decimal SaldoAnterior { get; set; }

    /// <summary>
    /// Saldo depois do movimento
    /// </summary>
    public decimal SaldoAtualizado { get; set; }

    /// <summary>
    /// Data/hora do movimento
    /// </summary>
    public string DataMovimento { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do tipo de movimento
    /// </summary>
    public string DescricaoTipo => TipoMovimento == 'C' ? "Crédito" : "Débito";

    /// <summary>
    /// Links HATEOAS
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
