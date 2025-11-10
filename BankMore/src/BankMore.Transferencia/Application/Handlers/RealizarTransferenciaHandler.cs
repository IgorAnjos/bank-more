using MediatR;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using BankMore.Transferencia.Application.Commands;
using BankMore.Transferencia.Application.DTOs;
using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;
using BankMore.Transferencia.Domain.Events;
using BankMore.Transferencia.Domain.Enums;
using System.Security.Cryptography;
using System.Text;

namespace BankMore.Transferencia.Application.Handlers;

/// <summary>
/// Handler para processar o comando de transferência
/// Implementa fluxo completo: débito origem → crédito destino → estorno se falhar
/// </summary>
public class RealizarTransferenciaHandler : IRequestHandler<RealizarTransferenciaCommand, TransferenciaDto>
{
    private readonly ITransferenciaRepository _transferenciaRepository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;
    private readonly ICompensacaoPendenteRepository _compensacaoPendenteRepository;
    private readonly IContaCorrenteService _contaCorrenteService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly decimal _valorTarifa;

    public RealizarTransferenciaHandler(
        ITransferenciaRepository transferenciaRepository,
        IIdempotenciaRepository idempotenciaRepository,
        ICompensacaoPendenteRepository compensacaoPendenteRepository,
        IContaCorrenteService contaCorrenteService,
        IKafkaProducerService kafkaProducerService,
        IConfiguration configuration)
    {
        _transferenciaRepository = transferenciaRepository ?? throw new ArgumentNullException(nameof(transferenciaRepository));
        _idempotenciaRepository = idempotenciaRepository ?? throw new ArgumentNullException(nameof(idempotenciaRepository));
        _compensacaoPendenteRepository = compensacaoPendenteRepository ?? throw new ArgumentNullException(nameof(compensacaoPendenteRepository));
        _contaCorrenteService = contaCorrenteService ?? throw new ArgumentNullException(nameof(contaCorrenteService));
        _kafkaProducerService = kafkaProducerService ?? throw new ArgumentNullException(nameof(kafkaProducerService));
        
        // Lê valor da tarifa do appsettings.json (default 2.00)
        var valorTarifaStr = configuration["Tarifas:ValorTransferencia"];
        _valorTarifa = !string.IsNullOrEmpty(valorTarifaStr) && decimal.TryParse(valorTarifaStr, out var valor) ? valor : 2.00m;
    }

    public async Task<TransferenciaDto> Handle(RealizarTransferenciaCommand request, CancellationToken cancellationToken)
    {
        // 1. Verificar idempotência
        var idempotenciaExistente = await _idempotenciaRepository.ObterPorChaveAsync(request.ChaveIdempotencia);
        if (idempotenciaExistente != null)
        {
            // Requisição duplicada - retornar resultado anterior
            // Em caso de idempotência, poderíamos buscar a transferência original
            // Por simplicidade, retornamos sucesso com informações básicas
            throw new InvalidOperationException("DUPLICATE_REQUEST: Requisição já processada anteriormente");
        }

        // 2. Validações
        if (request.Valor <= 0)
        {
            throw new InvalidOperationException("INVALID_VALUE: Valor deve ser maior que zero");
        }

        if (string.IsNullOrWhiteSpace(request.IdContaCorrenteOrigem))
        {
            throw new InvalidOperationException("INVALID_ACCOUNT: Conta origem inválida");
        }

        // 2.1. Validar se conta DESTINO existe e está ativa (SEM TRANSITAR DADOS SENSÍVEIS)
        // Usa apenas número da conta (não sensível) para validação
        var contaDestinoValida = await _contaCorrenteService.ValidarContaExistenteAsync(request.NumeroContaDestino);
        if (!contaDestinoValida)
        {
            throw new InvalidOperationException("INVALID_DESTINATION: Conta destino inválida ou inativa");
        }

        string chaveDebitoOrigem = $"{request.ChaveIdempotencia}-debito-origem";
        string chaveCreditoDestino = $"{request.ChaveIdempotencia}-credito-destino";
        string chaveEstorno = $"{request.ChaveIdempotencia}-estorno";

        bool debitoSucesso = false;
        bool creditoSucesso = false;
        Domain.Entities.Transferencia? transferencia = null;

        try
        {
            // 3. Realizar DÉBITO na conta origem
            debitoSucesso = await _contaCorrenteService.RealizarMovimentacaoAsync(
                request.Token,
                chaveDebitoOrigem,
                null, // null = usa conta do token
                'D', // Débito
                request.Valor
            );

            if (!debitoSucesso)
            {
                throw new InvalidOperationException("Falha ao debitar conta origem");
            }

            // 4. Realizar CRÉDITO na conta destino
            creditoSucesso = await _contaCorrenteService.RealizarMovimentacaoAsync(
                request.Token,
                chaveCreditoDestino,
                request.NumeroContaDestino,
                'C', // Crédito
                request.Valor
            );

            if (!creditoSucesso)
            {
                // 5. ESTORNAR o débito (rollback manual)
                await _contaCorrenteService.RealizarMovimentacaoAsync(
                    request.Token,
                    chaveEstorno,
                    null, // null = usa conta do token
                    'C', // Crédito de estorno
                    request.Valor
                );

                throw new InvalidOperationException("Falha ao creditar conta destino. Estorno realizado.");
            }

            // 6. Persistir transferência
            transferencia = new Domain.Entities.Transferencia(
                request.IdContaCorrenteOrigem,
                request.NumeroContaDestino.ToString(), // Simplificado - em produção buscar ID
                request.Valor
            );

            await _transferenciaRepository.AdicionarAsync(transferencia);

            // 7. Registrar idempotência com Status, Hash e Metadata
            var resultado = JsonSerializer.Serialize(new { transferencia.Id, transferencia.Valor, Status = "SUCESSO" });
            var resultadoHash = ComputeSha256Hash(resultado);
            var metadata = JsonSerializer.Serialize(new 
            { 
                ContaOrigem = request.IdContaCorrenteOrigem,
                ContaDestino = request.NumeroContaDestino,
                Valor = request.Valor,
                ChaveDebitoOrigem = chaveDebitoOrigem,
                ChaveCreditoDestino = chaveCreditoDestino
            });

            var idempotencia = new Idempotencia(
                request.ChaveIdempotencia,
                JsonSerializer.Serialize(request),
                resultado
            );
            idempotencia.AtualizarResultado(resultado, IdempotenciaStatus.Success, resultadoHash, metadata);
            await _idempotenciaRepository.AdicionarAsync(idempotencia);

            // 8. Publicar evento Kafka (assíncrono)
            try
            {
                var evento = new TransferenciaRealizadaEvent
                {
                    IdTransferencia = transferencia.Id,
                    IdContaCorrenteOrigem = transferencia.IdContaCorrenteOrigem,
                    IdContaCorrenteDestino = transferencia.IdContaCorrenteDestino,
                    Valor = transferencia.Valor,
                    DataMovimento = transferencia.DataMovimento,
                    ValorTarifa = _valorTarifa
                };

                await _kafkaProducerService.PublicarTransferenciaRealizadaAsync(evento);
            }
            catch (Exception kafkaEx)
            {
                // Log do erro Kafka mas não falha a transferência
                // Em produção usar ILogger
                Console.WriteLine($"Erro ao publicar evento Kafka: {kafkaEx.Message}");
            }

            // 9. Retornar DTO da transferência
            return new TransferenciaDto
            {
                Id = transferencia.Id,
                IdContaCorrenteOrigem = transferencia.IdContaCorrenteOrigem,
                IdContaCorrenteDestino = transferencia.IdContaCorrenteDestino,
                Valor = transferencia.Valor,
                DataTransferencia = transferencia.DataMovimento,
                Status = "Concluída"
            };
        }
        catch (Exception ex)
        {
            // ✅ SAGA PATTERN COM COMPENSAÇÃO GARANTIDA (Item 3 - duvidas.md)
            if (debitoSucesso && !creditoSucesso)
            {
                // Tentativa de estorno com retry exponencial (5 tentativas)
                bool estornoSucesso = await TentarEstornoComRetryAsync(
                    request.Token,
                    chaveEstorno,
                    request.Valor,
                    maxTentativas: 5
                );

                if (!estornoSucesso)
                {
                    // ❌ FALHA CRÍTICA: Estorno não funcionou após retries
                    // Registrar na fila de compensação para processamento assíncrono
                    var compensacao = new CompensacaoPendente(
                        transferencia?.Id ?? Guid.NewGuid().ToString(),
                        request.ChaveIdempotencia,
                        request.IdContaCorrenteOrigem,
                        request.Valor,
                        $"Falha no estorno após 5 tentativas. Erro original: {ex.Message}"
                    );

                    await _compensacaoPendenteRepository.AdicionarAsync(compensacao);

                    // TODO: Publicar no Kafka topic 'compensacoes-pendentes'
                    // await _kafkaProducerService.PublicarCompensacaoPendenteAsync(compensacao);

                    throw new InvalidOperationException(
                        "TRANSFER_FAILED_COMPENSATION_PENDING: Transferência falhou e estorno não pôde ser completado. " +
                        $"Compensação registrada para processamento assíncrono. ID: {compensacao.Id}",
                        ex
                    );
                }
            }

            throw new InvalidOperationException($"Erro ao processar transferência: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Tenta estorno com retry exponencial (backoff: 1s, 2s, 4s, 8s, 16s)
    /// </summary>
    private async Task<bool> TentarEstornoComRetryAsync(
        string token,
        string chaveEstorno,
        decimal valor,
        int maxTentativas = 5)
    {
        for (int tentativa = 1; tentativa <= maxTentativas; tentativa++)
        {
            try
            {
                Console.WriteLine($"[ESTORNO] Tentativa {tentativa}/{maxTentativas} - Valor: {valor}");

                bool sucesso = await _contaCorrenteService.RealizarMovimentacaoAsync(
                    token,
                    $"{chaveEstorno}-retry{tentativa}",
                    null,
                    'C',
                    valor
                );

                if (sucesso)
                {
                    Console.WriteLine($"[ESTORNO] ✅ Sucesso na tentativa {tentativa}");
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ESTORNO] ❌ Falha na tentativa {tentativa}: {ex.Message}");

                // Se não for a última tentativa, aguardar com backoff exponencial
                if (tentativa < maxTentativas)
                {
                    int delaySeconds = (int)Math.Pow(2, tentativa - 1); // 1, 2, 4, 8, 16
                    Console.WriteLine($"[ESTORNO] Aguardando {delaySeconds}s antes da próxima tentativa...");
                    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                }
            }
        }

        Console.WriteLine($"[ESTORNO] ❌ Todas as {maxTentativas} tentativas falharam");
        return false;
    }

    private static string ComputeSha256Hash(string rawData)
    {
        using var sha256Hash = SHA256.Create();
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
