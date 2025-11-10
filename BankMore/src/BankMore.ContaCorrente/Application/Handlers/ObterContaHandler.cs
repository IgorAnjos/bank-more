using MediatR;
using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Application.Handlers;

public class ObterContaHandler : IRequestHandler<ObterContaQuery, ContaDto?>
{
    private readonly ContaCorrenteDbContext _context;

    public ObterContaHandler(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ContaDto?> Handle(ObterContaQuery request, CancellationToken cancellationToken)
    {
        var conta = await _context.ContasCorrentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IdContaCorrente == request.IdContaCorrente, cancellationToken);

        if (conta == null)
            return null;

        // Calcular saldo
        var saldo = await _context.Movimentos
            .Where(m => m.IdContaCorrente == request.IdContaCorrente)
            .SumAsync(m => m.TipoMovimento == 'C' ? m.Valor : -m.Valor, cancellationToken);

        return new ContaDto
        {
            Id = conta.IdContaCorrente,
            NumeroContaCorrente = conta.Numero,
            Nome = conta.Nome,
            Ativo = conta.Ativo,
            Saldo = saldo,
            DataCriacao = DateTime.UtcNow
        };
    }
}
