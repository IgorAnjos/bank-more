using BankMore.Tarifas.Data;
using BankMore.Tarifas.Models;
using BankMore.Tarifas.Services;
using KafkaFlow;

namespace BankMore.Tarifas.Handlers;

/// <summary>
/// Handler para processar eventos de transferências realizadas do Kafka
/// </summary>
public class TransferenciaConsumerHandler : IMessageHandler<TransferenciaRealizadaEvent>
{
    private readonly ITarifaRepository _repository;
    private readonly IContaCorrenteService _contaService;
    private readonly ILogger<TransferenciaConsumerHandler> _logger;

    public TransferenciaConsumerHandler(
        ITarifaRepository repository,
        IContaCorrenteService contaService,
        ILogger<TransferenciaConsumerHandler> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _contaService = contaService ?? throw new ArgumentNullException(nameof(contaService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(IMessageContext context, TransferenciaRealizadaEvent message)
    {
        try
        {
            _logger.LogInformation(
                "Recebida transferência {IdTransferencia} - Origem: {Origem}, Destino: {Destino}, Tarifa: R$ {Tarifa}",
                message.IdTransferencia,
                message.IdContaCorrenteOrigem,
                message.IdContaCorrenteDestino,
                message.ValorTarifa);

            // Verificar idempotência (se já processamos esta transferência)
            if (await _repository.JaProcessadaAsync(message.IdTransferencia))
            {
                _logger.LogWarning("Tarifa já processada para transferência {IdTransferencia}", message.IdTransferencia);
                return;
            }

            // Criar registro da tarifa
            var tarifa = new Tarifa(
                message.IdContaCorrenteOrigem,
                message.IdTransferencia,
                message.ValorTarifa
            );

            // Persistir no banco de dados
            await _repository.AdicionarAsync(tarifa);

            _logger.LogInformation("Tarifa {IdTarifa} persistida com sucesso", tarifa.Id);

            // Debitar a tarifa na conta origem (chamada à API Conta)
            var debitoSucesso = await _contaService.DebitarTarifaAsync(
                message.IdContaCorrenteOrigem,
                message.IdTransferencia,
                message.ValorTarifa
            );

            if (!debitoSucesso)
            {
                _logger.LogError("Falha ao debitar tarifa da conta {IdConta}. Registro persistido mas débito não efetuado.", 
                    message.IdContaCorrenteOrigem);
                // Em produção: implementar retry policy ou dead letter queue
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar tarifa para transferência {IdTransferencia}", 
                message.IdTransferencia);
            throw; // Rejeitar mensagem para reprocessamento (depende da configuração do Kafka)
        }
    }
}
