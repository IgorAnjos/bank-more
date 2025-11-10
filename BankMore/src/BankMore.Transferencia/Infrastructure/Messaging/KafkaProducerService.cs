using BankMore.Transferencia.Domain.Events;
using BankMore.Transferencia.Domain.Interfaces;
using KafkaFlow.Producers;
using System.Text.Json;

namespace BankMore.Transferencia.Infrastructure.Messaging;

/// <summary>
/// Serviço para publicar eventos no Kafka
/// </summary>
public class KafkaProducerService : IKafkaProducerService
{
    private readonly IProducerAccessor _producerAccessor;

    public KafkaProducerService(IProducerAccessor producerAccessor)
    {
        _producerAccessor = producerAccessor ?? throw new ArgumentNullException(nameof(producerAccessor));
    }

    public async Task PublicarTransferenciaRealizadaAsync(TransferenciaRealizadaEvent evento)
    {
        var producer = _producerAccessor.GetProducer("transferencia-producer");
        
        var mensagem = JsonSerializer.Serialize(evento);
        
        await producer.ProduceAsync(
            topic: "transferencias-realizadas",
            messageKey: evento.IdTransferencia,
            messageValue: mensagem
        );
    }
}
