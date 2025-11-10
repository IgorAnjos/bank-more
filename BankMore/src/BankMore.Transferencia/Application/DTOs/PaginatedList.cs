namespace BankMore.Transferencia.Application.DTOs;

/// <summary>
/// Lista paginada de recursos
/// </summary>
public class PaginatedList<T>
{
    /// <summary>
    /// Itens da página atual
    /// </summary>
    public List<T> Items { get; set; } = new();

    /// <summary>
    /// Número da página atual (base 1)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// Tamanho da página
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// Total de itens
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Total de páginas
    /// </summary>
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);

    /// <summary>
    /// Indica se tem página anterior
    /// </summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>
    /// Indica se tem próxima página
    /// </summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>
    /// Links de navegação (HATEOAS)
    /// </summary>
    public List<Link> Links { get; set; } = new();
}
