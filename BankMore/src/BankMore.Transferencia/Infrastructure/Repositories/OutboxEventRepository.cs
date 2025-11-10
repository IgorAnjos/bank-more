using System.Data;
using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace BankMore.Transferencia.Infrastructure.Repositories;

public class OutboxEventRepository : IOutboxEventRepository
{
    private readonly string _connectionString;

    public OutboxEventRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new ArgumentNullException("ConnectionString não configurada");
    }

    private IDbConnection CreateConnection() => new SqliteConnection(_connectionString);

    public async Task<string> AdicionarAsync(OutboxEvent outboxEvent)
    {
        const string sql = @"
            INSERT INTO outbox_events (id, topic, event_type, payload, partition_key, created_at, processed, retry_count)
            VALUES (@Id, @Topic, @EventType, @Payload, @PartitionKey, @CreatedAt, @Processed, @RetryCount)";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            outboxEvent.Id,
            outboxEvent.Topic,
            outboxEvent.EventType,
            outboxEvent.Payload,
            outboxEvent.PartitionKey,
            CreatedAt = outboxEvent.CreatedAt.ToString("O"),
            Processed = outboxEvent.Processed ? 1 : 0,
            outboxEvent.RetryCount
        });

        return outboxEvent.Id;
    }

    public async Task<IEnumerable<OutboxEvent>> ObterNaoProcessadosAsync(int limit = 100)
    {
        const string sql = @"
            SELECT id, topic, event_type AS EventType, payload, partition_key AS PartitionKey,
                   created_at AS CreatedAt, processed_at AS ProcessedAt, processed,
                   retry_count AS RetryCount, error_message AS ErrorMessage
            FROM outbox_events
            WHERE processed = 0
            ORDER BY created_at
            LIMIT @Limit";

        using var connection = CreateConnection();
        var eventos = await connection.QueryAsync(sql, new { Limit = limit });

        return eventos.Select(MapToEntity);
    }

    public async Task AtualizarAsync(OutboxEvent outboxEvent)
    {
        const string sql = @"
            UPDATE outbox_events
            SET processed = @Processed,
                processed_at = @ProcessedAt,
                retry_count = @RetryCount,
                error_message = @ErrorMessage
            WHERE id = @Id";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            outboxEvent.Id,
            Processed = outboxEvent.Processed ? 1 : 0,
            ProcessedAt = outboxEvent.ProcessedAt?.ToString("O"),
            outboxEvent.RetryCount,
            outboxEvent.ErrorMessage
        });
    }

    public async Task MarcarComoProcessadoAsync(string id)
    {
        const string sql = @"
            UPDATE outbox_events
            SET processed = 1,
                processed_at = @ProcessedAt
            WHERE id = @Id";

        using var connection = CreateConnection();
        await connection.ExecuteAsync(sql, new
        {
            Id = id,
            ProcessedAt = DateTime.UtcNow.ToString("O")
        });
    }

    public async Task<int> RemoverProcessadosAntigosAsync(int diasRetencao = 7)
    {
        const string sql = @"
            DELETE FROM outbox_events
            WHERE processed = 1
              AND processed_at < @DataLimite";

        var dataLimite = DateTime.UtcNow.AddDays(-diasRetencao).ToString("O");

        using var connection = CreateConnection();
        return await connection.ExecuteAsync(sql, new { DataLimite = dataLimite });
    }

    private static OutboxEvent MapToEntity(dynamic row)
    {
        var outboxEvent = (OutboxEvent)Activator.CreateInstance(
            typeof(OutboxEvent),
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
            null,
            new object[] { (string)row.topic, (string)row.EventType, (string)row.payload, (string)row.PartitionKey },
            null)!;

        // Usa reflection para setar propriedades privadas
        var type = typeof(OutboxEvent);
        
        type.GetProperty("Id")!.SetValue(outboxEvent, (string)row.id);
        type.GetProperty("CreatedAt")!.SetValue(outboxEvent, DateTime.Parse((string)row.CreatedAt));
        type.GetProperty("Processed")!.SetValue(outboxEvent, row.processed == 1);
        type.GetProperty("RetryCount")!.SetValue(outboxEvent, (int)row.RetryCount);
        
        if (row.ProcessedAt != null)
            type.GetProperty("ProcessedAt")!.SetValue(outboxEvent, DateTime.Parse((string)row.ProcessedAt));
        
        if (row.ErrorMessage != null)
            type.GetProperty("ErrorMessage")!.SetValue(outboxEvent, (string)row.ErrorMessage);

        return outboxEvent;
    }
}
