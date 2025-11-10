using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

/// <summary>
/// Interface de repositório para Conta Corrente (padrão Repository Pattern)
/// </summary>
public interface IContaCorrenteRepository
{
    Task<Entities.ContaCorrente?> ObterPorIdAsync(string idContaCorrente);
    Task<Entities.ContaCorrente?> ObterPorNumeroAsync(int numero);
    Task<Entities.ContaCorrente?> ObterPorCpfAsync(string cpf); // CPF será armazenado no nome por enquanto
    Task<int> ObterProximoNumeroContaAsync();
    Task AdicionarAsync(Entities.ContaCorrente contaCorrente);
    Task AtualizarAsync(Entities.ContaCorrente contaCorrente);
    Task<bool> ExisteAsync(string idContaCorrente);
}
