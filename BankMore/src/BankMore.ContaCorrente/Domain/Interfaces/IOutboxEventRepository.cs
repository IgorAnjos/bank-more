using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

/// <summary>
/// Repositório para gerenciar eventos do Outbox Pattern
/// </summary>
public interface IOutboxEventRepository
{
    Task<string> AdicionarAsync(OutboxEvent outboxEvent);
    Task<IEnumerable<OutboxEvent>> ObterNaoProcessadosAsync(int limit = 100);
    Task AtualizarAsync(OutboxEvent outboxEvent);
    Task MarcarComoProcessadoAsync(string id);
    Task<int> RemoverProcessadosAntigosAsync(int diasRetencao = 7);
}
