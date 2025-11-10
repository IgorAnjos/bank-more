using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Movimentos
/// </summary>
public interface IMovimentoRepository
{
    Task AdicionarAsync(Movimento movimento);
    Task<IEnumerable<Movimento>> ObterPorContaAsync(string idContaCorrente);
    Task<decimal> CalcularSaldoAsync(string idContaCorrente);
}
