using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

/// <summary>
/// Implementação do Unit of Work para gerenciamento de transações
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ContaCorrenteDbContext _context;

    public UnitOfWork(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> CommitAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task RollbackAsync()
    {
        await Task.CompletedTask;
        // O EF Core não persiste mudanças até o SaveChanges
        // então não há necessidade de rollback explícito
    }

    public void Dispose()
    {
        _context?.Dispose();
    }
}
