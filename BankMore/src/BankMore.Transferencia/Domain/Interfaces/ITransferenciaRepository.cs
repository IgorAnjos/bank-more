using BankMore.Transferencia.Domain.Entities;

namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Transferências usando Dapper
/// </summary>
public interface ITransferenciaRepository
{
    Task<Entities.Transferencia?> ObterPorIdAsync(string idTransferencia);
    Task<string> AdicionarAsync(Entities.Transferencia transferencia);
    Task<IEnumerable<Entities.Transferencia>> ObterPorContaOrigemAsync(string idContaCorrenteOrigem);
}
