using Microsoft.EntityFrameworkCore;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Data;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Conta Corrente usando Entity Framework Core
/// </summary>
public class ContaCorrenteRepository : IContaCorrenteRepository
{
    private readonly ContaCorrenteDbContext _context;

    public ContaCorrenteRepository(ContaCorrenteDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.Entities.ContaCorrente?> ObterPorIdAsync(string idContaCorrente)
    {
        return await _context.ContasCorrentes
            .Include(c => c.Movimentos)
            .FirstOrDefaultAsync(c => c.IdContaCorrente == idContaCorrente);
    }

    public async Task<Domain.Entities.ContaCorrente?> ObterPorNumeroAsync(int numero)
    {
        return await _context.ContasCorrentes
            .Include(c => c.Movimentos)
            .FirstOrDefaultAsync(c => c.Numero == numero);
    }

    public async Task<Domain.Entities.ContaCorrente?> ObterPorCpfAsync(string cpf)
    {
        // CPF está criptografado no banco, então o parâmetro 'cpf' já deve vir criptografado
        // A busca é feita diretamente no campo Cpf que contém o valor criptografado
        return await _context.ContasCorrentes
            .Include(c => c.Movimentos)
            .FirstOrDefaultAsync(c => c.Cpf == cpf);
    }

    public async Task<int> ObterProximoNumeroContaAsync()
    {
        var ultimoNumero = await _context.ContasCorrentes
            .MaxAsync(c => (int?)c.Numero) ?? 0;
        
        return ultimoNumero + 1;
    }

    public async Task AdicionarAsync(Domain.Entities.ContaCorrente contaCorrente)
    {
        await _context.ContasCorrentes.AddAsync(contaCorrente);
    }

    public async Task AtualizarAsync(Domain.Entities.ContaCorrente contaCorrente)
    {
        _context.ContasCorrentes.Update(contaCorrente);
        await Task.CompletedTask;
    }

    public async Task<bool> ExisteAsync(string idContaCorrente)
    {
        return await _context.ContasCorrentes
            .AnyAsync(c => c.IdContaCorrente == idContaCorrente);
    }
}
