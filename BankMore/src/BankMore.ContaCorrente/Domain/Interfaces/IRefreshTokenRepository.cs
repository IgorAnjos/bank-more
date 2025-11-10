using BankMore.ContaCorrente.Domain.Entities;

namespace BankMore.ContaCorrente.Domain.Interfaces;

/// <summary>
/// Repositório para gerenciamento de Refresh Tokens
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> ObterPorTokenHashAsync(string tokenHash);
    Task<RefreshToken?> ObterPorIdAsync(string id);
    Task<List<RefreshToken>> ObterPorContaAsync(string idContaCorrente);
    Task AdicionarAsync(RefreshToken refreshToken);
    Task AtualizarAsync(RefreshToken refreshToken);
    Task RevogarTodosPorContaAsync(string idContaCorrente, string motivo);
}
