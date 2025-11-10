namespace BankMore.ContaCorrente.Application.DTOs;

/// <summary>
/// DTO para resposta de conta com HATEOAS
/// </summary>
public class ContaDto : IHateoasResource
{
    /// <summary>
    /// ID único da conta corrente
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta corrente
    /// </summary>
    public int NumeroContaCorrente { get; set; }

    /// <summary>
    /// CPF do titular (apenas em respostas específicas, não exposto em listagens)
    /// </summary>
    public string? Cpf { get; set; }

    /// <summary>
    /// Nome do titular da conta
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Status da conta (ativo/inativo)
    /// </summary>
    public bool Ativo { get; set; }

    /// <summary>
    /// Saldo atual da conta
    /// </summary>
    public decimal? Saldo { get; set; }

    /// <summary>
    /// Data de criação da conta
    /// </summary>
    public DateTime DataCriacao { get; set; }

    /// <summary>
    /// Links HATEOAS para navegação
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
