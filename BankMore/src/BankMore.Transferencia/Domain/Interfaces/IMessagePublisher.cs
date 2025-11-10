namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface para publicação de eventos em sistema de mensageria (Kafka)
/// </summary>
public interface IMessagePublisher
{
    /// <summary>
    /// Publica um evento em um tópico específico
    /// </summary>
    /// <typeparam name="TEvent">Tipo do evento</typeparam>
    /// <param name="topic">Nome do tópico Kafka</param>
    /// <param name="event">Evento a ser publicado</param>
    /// <param name="partitionKey">Chave de partição (ex: IdContaCorrente) para garantir ordem</param>
    Task PublishAsync<TEvent>(string topic, TEvent @event, string? partitionKey = null) where TEvent : class;

    /// <summary>
    /// Publica múltiplos eventos em um tópico de forma transacional
    /// </summary>
    Task PublishBatchAsync<TEvent>(string topic, IEnumerable<TEvent> events, string? partitionKey = null) where TEvent : class;
}
