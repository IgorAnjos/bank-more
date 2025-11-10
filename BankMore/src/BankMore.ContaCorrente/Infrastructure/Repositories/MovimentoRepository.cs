using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Movimento usando Entity Framework Core
/// </summary>
public class MovimentoRepository : IMovimentoRepository
{
    private readonly ContaCorrenteDbContext _context;

    public MovimentoRepository(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task AdicionarAsync(Movimento movimento)
    {
        await _context.Movimentos.AddAsync(movimento);
    }

    public async Task<IEnumerable<Movimento>> ObterPorContaAsync(string idContaCorrente)
    {
        return await _context.Movimentos
            .Where(m => m.IdContaCorrente == idContaCorrente)
            .OrderByDescending(m => m.DataMovimento)
            .ToListAsync();
    }

    public async Task<decimal> CalcularSaldoAsync(string idContaCorrente)
    {
        var movimentos = await _context.Movimentos
            .Where(m => m.IdContaCorrente == idContaCorrente)
            .ToListAsync();

        if (!movimentos.Any())
            return 0.00m;

        var creditos = movimentos
            .Where(m => m.TipoMovimento == 'C')
            .Sum(m => m.Valor);

        var debitos = movimentos
            .Where(m => m.TipoMovimento == 'D')
            .Sum(m => m.Valor);

        return Math.Round(creditos - debitos, 2);
    }
}
