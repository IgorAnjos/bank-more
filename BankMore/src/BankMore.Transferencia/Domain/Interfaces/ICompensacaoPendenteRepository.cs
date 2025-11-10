using BankMore.Transferencia.Domain.Entities;

namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Repositório para gerenciamento de compensações pendentes
/// </summary>
public interface ICompensacaoPendenteRepository
{
    Task<CompensacaoPendente?> ObterPorIdAsync(string id);
    Task<CompensacaoPendente?> ObterPorIdTransferenciaAsync(string idTransferencia);
    Task<List<CompensacaoPendente>> ObterPendentesAsync(int limite = 100);
    Task AdicionarAsync(CompensacaoPendente compensacao);
    Task AtualizarAsync(CompensacaoPendente compensacao);
    Task<int> ContarPendentesAsync();
}
