namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface para Unit of Work (gerenciamento de transações com Dapper)
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}
