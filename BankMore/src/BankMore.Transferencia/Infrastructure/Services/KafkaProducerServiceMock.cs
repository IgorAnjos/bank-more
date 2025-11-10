using BankMore.Transferencia.Domain.Events;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Services;

/// <summary>
/// Mock do Kafka Producer para testes sem broker
/// </summary>
public class KafkaProducerServiceMock : IKafkaProducerService
{
    public Task PublicarTransferenciaRealizadaAsync(TransferenciaRealizadaEvent evento)
    {
        // Mock: apenas loga que o evento seria publicado
        Console.WriteLine($"[MOCK] Kafka - Transferência {evento.IdTransferencia} seria publicada (valor: {evento.Valor}, tarifa: {evento.ValorTarifa})");
        return Task.CompletedTask;
    }
}
