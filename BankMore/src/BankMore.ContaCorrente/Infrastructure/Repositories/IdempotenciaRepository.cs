using System.Text.Json;
using StackExchange.Redis;
using BankMore.ContaCorrente.Domain.Entities;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Domain.Enums;

namespace BankMore.ContaCorrente.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Idempotência usando Redis
/// Armazena dados com TTL automático (24 horas configurável)
/// </summary>
public class IdempotenciaRepository : IIdempotenciaRepository
{
    private readonly IDatabase _redis;
    private readonly TimeSpan _ttl;

    public IdempotenciaRepository(IConnectionMultiplexer redis, TimeSpan ttl)
    {
        _redis = redis?.GetDatabase() ?? throw new ArgumentNullException(nameof(redis));
        _ttl = ttl;
    }

    public async Task<Idempotencia?> ObterPorChaveAsync(string chaveIdempotencia)
    {
        var json = await _redis.StringGetAsync(chaveIdempotencia);
        
        if (json.IsNullOrEmpty)
            return null;

        var data = JsonSerializer.Deserialize<IdempotenciaRedisDto>(json.ToString());
        if (data == null)
            return null;

        // Reconstruir entidade a partir do JSON
        var idempotencia = new Idempotencia(data.ChaveIdempotencia, data.Requisicao, data.Resultado);
        idempotencia.AtualizarResultado(
            data.Resultado, 
            data.Status, 
            data.ResultadoHash, 
            data.Metadata
        );

        return idempotencia;
    }

    public async Task AdicionarAsync(Idempotencia idempotencia)
    {
        var dto = new IdempotenciaRedisDto
        {
            ChaveIdempotencia = idempotencia.ChaveIdempotencia,
            Requisicao = idempotencia.Requisicao,
            Resultado = idempotencia.Resultado,
            Status = idempotencia.Status,
            ResultadoHash = idempotencia.ResultadoHash,
            Metadata = idempotencia.Metadata,
            DataCriacao = idempotencia.DataCriacao,
            DataExpiracao = idempotencia.DataExpiracao
        };

        var json = JsonSerializer.Serialize(dto);
        await _redis.StringSetAsync(idempotencia.ChaveIdempotencia, json, _ttl);
    }

    public async Task<bool> ExisteAsync(string chaveIdempotencia)
    {
        return await _redis.KeyExistsAsync(chaveIdempotencia);
    }

    /// <summary>
    /// DTO para serialização no Redis
    /// </summary>
    private class IdempotenciaRedisDto
    {
        public string ChaveIdempotencia { get; set; } = string.Empty;
        public string Requisicao { get; set; } = string.Empty;
        public string Resultado { get; set; } = string.Empty;
        public IdempotenciaStatus Status { get; set; }
        public string? ResultadoHash { get; set; }
        public string? Metadata { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime DataExpiracao { get; set; }
    }
}
