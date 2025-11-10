using BankMore.Transferencia.Domain.Entities;

namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface de repositório para controle de Idempotência
/// </summary>
public interface IIdempotenciaRepository
{
    Task<Idempotencia?> ObterPorChaveAsync(string chaveIdempotencia);
    Task AdicionarAsync(Idempotencia idempotencia);
    Task<bool> ExisteAsync(string chaveIdempotencia);
}
