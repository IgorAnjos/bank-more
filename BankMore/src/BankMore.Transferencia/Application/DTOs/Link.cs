namespace BankMore.Transferencia.Application.DTOs;

/// <summary>
/// Link HATEOAS para navegação entre recursos
/// </summary>
public class Link
{
    /// <summary>
    /// Tipo de relação (self, conta-origem, conta-destino, etc)
    /// </summary>
    public string Rel { get; set; } = string.Empty;

    /// <summary>
    /// URI do recurso
    /// </summary>
    public string Href { get; set; } = string.Empty;

    /// <summary>
    /// Método HTTP (GET, POST, PUT, DELETE, PATCH)
    /// </summary>
    public string Method { get; set; } = "GET";

    /// <summary>
    /// Descrição opcional do link
    /// </summary>
    public string? Description { get; set; }
}

/// <summary>
/// Interface para DTOs que suportam HATEOAS
/// </summary>
public interface IHateoasResource
{
    List<Link> Links { get; set; }
}
