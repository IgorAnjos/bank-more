namespace BankMore.Tarifas;

/// <summary>
/// Worker Service que mantém o consumidor Kafka ativo
/// O consumo de mensagens é feito via KafkaFlow configurado no Program.cs
/// </summary>
public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker de Tarifas iniciado. Aguardando mensagens do Kafka...");

        // O KafkaFlow gerencia o consumo das mensagens em background
        // Este worker apenas mantém o serviço ativo
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }

        logger.LogInformation("Worker de Tarifas encerrado");
    }
}

