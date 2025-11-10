namespace BankMore.Transferencia.Domain.Entities;

/// <summary>
/// Entidade para Outbox Pattern - garante consistência entre banco de dados e Kafka
/// Eventos são salvos no banco e processados de forma assíncrona
/// </summary>
public class OutboxEvent
{
    public string Id { get; private set; }
    public string Topic { get; private set; }
    public string EventType { get; private set; }
    public string Payload { get; private set; } // JSON serializado
    public string? PartitionKey { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? ProcessedAt { get; private set; }
    public bool Processed { get; private set; }
    public int RetryCount { get; private set; }
    public string? ErrorMessage { get; private set; }

    // Construtor privado para EF Core
    private OutboxEvent()
    {
        Id = string.Empty;
        Topic = string.Empty;
        EventType = string.Empty;
        Payload = string.Empty;
    }

    public OutboxEvent(string topic, string eventType, string payload, string? partitionKey = null)
    {
        if (string.IsNullOrWhiteSpace(topic))
            throw new ArgumentNullException(nameof(topic));
        
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentNullException(nameof(eventType));
        
        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentNullException(nameof(payload));

        Id = Guid.NewGuid().ToString();
        Topic = topic;
        EventType = eventType;
        Payload = payload;
        PartitionKey = partitionKey;
        CreatedAt = DateTime.UtcNow;
        Processed = false;
        RetryCount = 0;
    }

    public void MarkAsProcessed()
    {
        Processed = true;
        ProcessedAt = DateTime.UtcNow;
    }

    public void IncrementRetry(string errorMessage)
    {
        RetryCount++;
        ErrorMessage = errorMessage;
    }

    public bool CanRetry() => RetryCount < 5;
}
