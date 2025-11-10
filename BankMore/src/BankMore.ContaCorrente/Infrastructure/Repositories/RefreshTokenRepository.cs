using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Refresh Tokens usando Entity Framework Core
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ContaCorrenteDbContext _context;

    public RefreshTokenRepository(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<RefreshToken?> ObterPorTokenHashAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
    }

    public async Task<RefreshToken?> ObterPorIdAsync(string id)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Id == id);
    }

    public async Task<List<RefreshToken>> ObterPorContaAsync(string idContaCorrente)
    {
        return await _context.RefreshTokens
            .Where(rt => rt.IdContaCorrente == idContaCorrente)
            .OrderByDescending(rt => rt.DataCriacao)
            .ToListAsync();
    }

    public async Task AdicionarAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task AtualizarAsync(RefreshToken refreshToken)
    {
        _context.RefreshTokens.Update(refreshToken);
    }

    public async Task RevogarTodosPorContaAsync(string idContaCorrente, string motivo)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.IdContaCorrente == idContaCorrente && !rt.Revogado)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revogar(motivo);
        }
    }
}
