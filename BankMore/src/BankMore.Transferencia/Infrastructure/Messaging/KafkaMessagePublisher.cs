using System.Text.Json;
using Confluent.Kafka;
using BankMore.Transferencia.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BankMore.Transferencia.Infrastructure.Messaging;

/// <summary>
/// Implementação do publisher Kafka com suporte a retry, partition key e idempotência
/// </summary>
public class KafkaMessagePublisher : IMessagePublisher, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaMessagePublisher> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaMessagePublisher(IConfiguration configuration, ILogger<KafkaMessagePublisher> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            ClientId = configuration["Kafka:ClientId"] ?? "bankmore-transferencia",
            
            // Configurações de confiabilidade
            Acks = Acks.All, // Aguarda confirmação de todas as réplicas
            EnableIdempotence = true, // Garante que mensagens não serão duplicadas
            MaxInFlight = 5, // Máximo de requisições em paralelo
            MessageSendMaxRetries = 3, // Retry automático
            RetryBackoffMs = 100,
            
            // Configurações de performance
            CompressionType = CompressionType.Snappy,
            BatchSize = 16384,
            LingerMs = 10
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task PublishAsync<TEvent>(string topic, TEvent @event, string? partitionKey = null) where TEvent : class
    {
        try
        {
            var json = JsonSerializer.Serialize(@event, _jsonOptions);
            var key = partitionKey ?? Guid.NewGuid().ToString();

            var message = new Message<string, string>
            {
                Key = key,
                Value = json,
                Headers = new Headers
                {
                    { "event-type", System.Text.Encoding.UTF8.GetBytes(typeof(TEvent).Name) },
                    { "published-at", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
                }
            };

            var result = await _producer.ProduceAsync(topic, message);

            _logger.LogInformation(
                "Evento publicado com sucesso. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}",
                topic, result.Partition.Value, result.Offset.Value, key);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, 
                "Erro ao publicar evento. Topic: {Topic}, Error: {ErrorCode} - {ErrorReason}",
                topic, ex.Error.Code, ex.Error.Reason);
            throw;
        }
    }

    public async Task PublishBatchAsync<TEvent>(string topic, IEnumerable<TEvent> events, string? partitionKey = null) where TEvent : class
    {
        var tasks = events.Select(e => PublishAsync(topic, e, partitionKey));
        await Task.WhenAll(tasks);
    }

    public void Dispose()
    {
        _producer?.Flush(TimeSpan.FromSeconds(10));
        _producer?.Dispose();
    }
}
