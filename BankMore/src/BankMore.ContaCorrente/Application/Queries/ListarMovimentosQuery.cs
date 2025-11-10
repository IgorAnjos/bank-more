using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Queries;

/// <summary>
/// Query para listar movimentos com paginação e filtros
/// </summary>
public class ListarMovimentosQuery : IRequest<PaginatedList<MovimentoDto>>
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public char? TipoMovimento { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
