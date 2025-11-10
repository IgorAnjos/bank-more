using MediatR;
using BankMore.Transferencia.Application.DTOs;

namespace BankMore.Transferencia.Application.Queries;

/// <summary>
/// Query para listar transferências com paginação e filtros
/// </summary>
public class ListarTransferenciasQuery : IRequest<PaginatedList<TransferenciaDto>>
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
    public string? Tipo { get; set; } // "enviadas", "recebidas", "todas"
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
