using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Domain.Enums;
using BankMore.ContaCorrente.Infrastructure.Services;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar o comando de movimentação
/// Implementa idempotência conforme especificação
/// </summary>
public class MovimentacaoHandler : IRequestHandler<MovimentacaoCommand, bool>
{
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IMovimentoRepository _movimentoRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;
    private readonly IUnitOfWork _unitOfWork;

    public MovimentacaoHandler(
        IContaCorrenteRepository contaRepository,
        IMovimentoRepository movimentoRepository,
        IIdempotenciaRepository idempotenciaRepository,
        IUnitOfWork unitOfWork)
    {
        _contaRepository = contaRepository ?? throw new ArgumentNullException(nameof(contaRepository));
        _movimentoRepository = movimentoRepository ?? throw new ArgumentNullException(nameof(movimentoRepository));
        _idempotenciaRepository = idempotenciaRepository ?? throw new ArgumentNullException(nameof(idempotenciaRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(MovimentacaoCommand request, CancellationToken cancellationToken)
    {
        // Verificar idempotência
        var idempotenciaExistente = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
        if (idempotenciaExistente != null)
        {
            // Requisição duplicada, retorna o resultado anterior
            return true;
        }

        // Determinar qual conta será movimentada
        string idContaMovimentacao = request.IdContaCorrente; // Do token JWT
        
        if (request.NumeroConta.HasValue)
        {
            var contaDestino = await _contaRepository.ObterPorNumeroAsync(request.NumeroConta.Value);
            if (contaDestino == null)
            {
                throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada");
            }
            idContaMovimentacao = contaDestino.IdContaCorrente;
        }

        // Buscar a conta
        var conta = await _contaRepository.ObterPorIdAsync(idContaMovimentacao);
        
        if (conta == null)
        {
            throw new InvalidOperationException("INVALID_ACCOUNT: Conta não cadastrada");
        }

        if (!conta.Ativo)
        {
            throw new InvalidOperationException("INACTIVE_ACCOUNT: Conta inativa");
        }

        // Validar tipo de movimento
        if (request.TipoMovimento != 'C' && request.TipoMovimento != 'D')
        {
            throw new InvalidOperationException("INVALID_TYPE: Tipo de movimento inválido");
        }

        // Validar valor
        if (request.Valor <= 0)
        {
            throw new InvalidOperationException("INVALID_VALUE: Valor deve ser maior que zero");
        }

        // Validar débito apenas na própria conta
        if (request.TipoMovimento == 'D' && idContaMovimentacao != request.IdContaCorrente)
        {
            throw new InvalidOperationException("INVALID_TYPE: Débito só pode ser realizado na própria conta");
        }

        // ✅ VALIDAÇÃO DE SALDO ANTES DE DÉBITO (Item 3 - duvidas.md)
        if (request.TipoMovimento == 'D')
        {
            var saldoAtual = await _movimentoRepository.CalcularSaldoAsync(idContaMovimentacao);
            
            if (saldoAtual < request.Valor)
            {
                throw new InvalidOperationException($"INSUFFICIENT_BALANCE: Saldo insuficiente. Disponível: {saldoAtual:F2}, Solicitado: {request.Valor:F2}");
            }
        }

        // Criar o movimento dentro de transação
        var movimento = new Movimento(idContaMovimentacao, request.TipoMovimento, request.Valor);
        
        // Persistir movimento
        await _movimentoRepository.AdicionarAsync(movimento);

        // ✅ RECALCULAR E VALIDAR SALDO APÓS INSERÇÃO (Lock Otimista)
        if (request.TipoMovimento == 'D')
        {
            var saldoFinal = await _movimentoRepository.CalcularSaldoAsync(idContaMovimentacao);
            
            if (saldoFinal < 0)
            {
                // Rollback via exceção - UnitOfWork não fará commit
                throw new InvalidOperationException($"INSUFFICIENT_BALANCE: Saldo ficaria negativo após operação. Saldo final: {saldoFinal:F2}");
            }
        }

        // Registrar idempotência com status Success
        var resultado = JsonSerializer.Serialize(new { MovimentoId = movimento.IdMovimento, Sucesso = true });
        var resultadoHash = ComputeSha256Hash(resultado);
        var metadata = JsonSerializer.Serialize(new 
        { 
            ContaId = idContaMovimentacao,
            TipoMovimento = request.TipoMovimento,
            Valor = request.Valor,
            NumeroConta = request.NumeroConta
        });

        var idempotencia = new Idempotencia(
            request.ChaveIdempotencia,
            JsonSerializer.Serialize(request),
            resultado
        );
        idempotencia.AtualizarResultado(resultado, IdempotenciaStatus.Success, resultadoHash, metadata);
        await _idempotenciaRepository.AdicionarAsync(idempotencia);

        await _unitOfWork.CommitAsync();

        return true;
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
