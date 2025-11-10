using MediatR;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Application.Handlers;

public class ListarMovimentosHandler : IRequestHandler<ListarMovimentosQuery, PaginatedList<MovimentoDto>>
{
    private readonly ContaCorrenteDbContext _context;

    public ListarMovimentosHandler(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<PaginatedList<MovimentoDto>> Handle(ListarMovimentosQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Movimentos
            .AsNoTracking()
            .Where(m => m.IdContaCorrente == request.IdContaCorrente);

        // Aplicar filtros
        if (request.TipoMovimento.HasValue)
        {
            query = query.Where(m => m.TipoMovimento == request.TipoMovimento.Value);
        }

        if (request.DataInicio.HasValue)
        {
            var dataInicioStr = request.DataInicio.Value.ToString("dd/MM/yyyy");
            query = query.Where(m => string.Compare(m.DataMovimento, dataInicioStr) >= 0);
        }

        if (request.DataFim.HasValue)
        {
            var dataFimStr = request.DataFim.Value.ToString("dd/MM/yyyy");
            query = query.Where(m => string.Compare(m.DataMovimento, dataFimStr) <= 0);
        }

        // Contar total
        var totalItems = await query.CountAsync(cancellationToken);

        // Aplicar paginação
        var items = await query
            .OrderByDescending(m => m.DataMovimento)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MovimentoDto
            {
                Id = m.IdMovimento,
                IdContaCorrente = m.IdContaCorrente,
                TipoMovimento = m.TipoMovimento,
                Valor = m.Valor,
                DataMovimento = m.DataMovimento
            })
            .ToListAsync(cancellationToken);

        return new PaginatedList<MovimentoDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems
        };
    }
}
