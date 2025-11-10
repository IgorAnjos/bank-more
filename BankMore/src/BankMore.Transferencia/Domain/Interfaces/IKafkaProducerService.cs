using BankMore.Transferencia.Domain.Events;

namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface para serviço de publicação de eventos no Kafka
/// </summary>
public interface IKafkaProducerService
{
    Task PublicarTransferenciaRealizadaAsync(TransferenciaRealizadaEvent evento);
}
