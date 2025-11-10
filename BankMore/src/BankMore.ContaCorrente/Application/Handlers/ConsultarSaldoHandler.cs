using MediatR;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar a query de consulta de saldo
/// Padrão CQRS - separação de leitura
/// </summary>
public class ConsultarSaldoHandler : IRequestHandler<ConsultarSaldoQuery, SaldoDto>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;

    public ConsultarSaldoHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository)
    {
        _contaRepository = contaRepository ?? throw new ArgumentNullException(nameof(contaRepository));
        _movimentoRepository = movimentoRepository ?? throw new ArgumentNullException(nameof(movimentoRepository));
    }

    public async Task<SaldoDto> Handle(ConsultarSaldoQuery request, CancellationToken cancellationToken)
    {
        // Buscar a conta
        var conta = await _contaRepository.ObterPorIdAsync(request.IdContaCorrente);

        if (conta == null)
        {
            throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada");
        }

        if (!conta.Ativo)
        {
            throw new InvalidOperationException("INACTIVE_ACCOUNT: Conta inativa");
        }

        // Calcular saldo
        var saldo = await _movimentoRepository.CalcularSaldoAsync(request.IdContaCorrente);

        return new SaldoDto
        {
            NumeroConta = conta.Numero,
            NomeTitular = conta.Nome,
            DataHoraConsulta = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"),
            Saldo = saldo
        };
    }
}
