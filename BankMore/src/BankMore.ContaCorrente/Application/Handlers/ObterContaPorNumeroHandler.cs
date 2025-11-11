using MediatR;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Infrastructure.Data;
using BankMore.ContaCorrente.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para buscar conta corrente por número
/// </summary>
public class ObterContaPorNumeroHandler : IRequestHandler<ObterContaPorNumeroQuery, ContaDto?>
{
    private readonly ContaCorrenteDbContext _context;
    private readonly ICryptographyService _cryptographyService;

    public ObterContaPorNumeroHandler(ContaCorrenteDbContext context, ICryptographyService cryptographyService)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _cryptographyService = cryptographyService ?? throw new ArgumentNullException(nameof(cryptographyService));
    }

    public async Task<ContaDto?> Handle(ObterContaPorNumeroQuery request, CancellationToken cancellationToken)
    {
        var conta = await _context.ContasCorrentes
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Numero == request.NumeroConta, cancellationToken);

        if (conta == null)
            return null;

        // Descriptografar CPF antes de retornar
        var cpfDescriptografado = _cryptographyService.Decrypt(conta.Cpf);

        return new ContaDto
        {
            Id = conta.IdContaCorrente,
            NumeroContaCorrente = conta.Numero,
            Cpf = cpfDescriptografado,
            Nome = conta.Nome,
            Ativo = conta.Ativo,
            DataCriacao = DateTime.UtcNow
        };
    }
}
