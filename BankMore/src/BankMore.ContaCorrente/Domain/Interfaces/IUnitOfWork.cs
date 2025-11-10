namespace BankMore.ContaCorrente.Domain.Interfaces;

/// <summary>
/// Interface para Unit of Work (gerenciamento de transações)
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task<int> CommitAsync();
    Task RollbackAsync();
}
