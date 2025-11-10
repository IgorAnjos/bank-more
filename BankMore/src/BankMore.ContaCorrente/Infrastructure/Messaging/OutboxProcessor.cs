using BankMore.ContaCorrente.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BankMore.ContaCorrente.Infrastructure.Messaging;

/// <summary>
/// Background Service que processa eventos do Outbox Pattern
/// Garante consistência eventual entre banco de dados e Kafka
/// </summary>
public class OutboxProcessor : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessor> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5); // Intervalo de processamento

    public OutboxProcessor(IServiceProvider serviceProvider, ILogger<OutboxProcessor> logger)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxProcessor iniciado");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxEventsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar eventos do outbox");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.LogInformation("OutboxProcessor finalizado");
    }

    private async Task ProcessOutboxEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxEventRepository>();
        var messagePublisher = scope.ServiceProvider.GetRequiredService<IMessagePublisher>();

        var eventos = await outboxRepository.ObterNaoProcessadosAsync(100);

        foreach (var evento in eventos)
        {
            if (cancellationToken.IsCancellationRequested)
                break;

            try
            {
                // Publica o evento no Kafka
                await messagePublisher.PublishAsync(
                    evento.Topic,
                    evento.Payload, // Já está serializado como JSON
                    evento.PartitionKey
                );

                // Marca como processado
                evento.MarkAsProcessed();
                await outboxRepository.AtualizarAsync(evento);

                _logger.LogInformation(
                    "Evento processado com sucesso. Id: {EventoId}, Topic: {Topic}",
                    evento.Id, evento.Topic);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Erro ao processar evento. Id: {EventoId}, Topic: {Topic}, Tentativa: {RetryCount}",
                    evento.Id, evento.Topic, evento.RetryCount);

                evento.IncrementRetry(ex.Message);
                await outboxRepository.AtualizarAsync(evento);

                if (!evento.CanRetry())
                {
                    _logger.LogWarning(
                        "Evento atingiu limite de retries. Id: {EventoId}, Topic: {Topic}. Movendo para DLQ manual.",
                        evento.Id, evento.Topic);
                }
            }
        }

        // Limpa eventos antigos processados (mantém 7 dias de histórico)
        if (DateTime.UtcNow.Hour == 3 && DateTime.UtcNow.Minute < 10) // Executa às 3h da manhã
        {
            var removidos = await outboxRepository.RemoverProcessadosAntigosAsync(7);
            if (removidos > 0)
            {
                _logger.LogInformation("Removidos {Count} eventos processados antigos", removidos);
            }
        }
    }
}
