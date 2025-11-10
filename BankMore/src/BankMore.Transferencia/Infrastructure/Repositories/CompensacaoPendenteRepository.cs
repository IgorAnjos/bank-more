using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Compensações Pendentes usando Dapper
/// </summary>
public class CompensacaoPendenteRepository : ICompensacaoPendenteRepository
{
    private readonly string _connectionString;

    public CompensacaoPendenteRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<CompensacaoPendente?> ObterPorIdAsync(string id)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                id_transferencia AS IdTransferencia,
                chave_idempotencia AS ChaveIdempotencia,
                id_conta_origem AS IdContaOrigem,
                valor_estorno AS ValorEstorno,
                tentativas_realizadas AS TentativasRealizadas,
                data_criacao AS DataCriacao,
                data_ultima_retentativa AS DataUltimaRetentativa,
                data_resolucao AS DataResolucao,
                status AS Status,
                motivo_falha AS MotivoFalha,
                observacoes_operador AS ObservacoesOperador
            FROM compensacao_pendente
            WHERE id = @Id";

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<CompensacaoPendente>(sql, new { Id = id });
    }

    public async Task<CompensacaoPendente?> ObterPorIdTransferenciaAsync(string idTransferencia)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                id_transferencia AS IdTransferencia,
                chave_idempotencia AS ChaveIdempotencia,
                id_conta_origem AS IdContaOrigem,
                valor_estorno AS ValorEstorno,
                tentativas_realizadas AS TentativasRealizadas,
                data_criacao AS DataCriacao,
                data_ultima_retentativa AS DataUltimaRetentativa,
                data_resolucao AS DataResolucao,
                status AS Status,
                motivo_falha AS MotivoFalha,
                observacoes_operador AS ObservacoesOperador
            FROM compensacao_pendente
            WHERE id_transferencia = @IdTransferencia";

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<CompensacaoPendente>(sql, new { IdTransferencia = idTransferencia });
    }

    public async Task<List<CompensacaoPendente>> ObterPendentesAsync(int limite = 100)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                id_transferencia AS IdTransferencia,
                chave_idempotencia AS ChaveIdempotencia,
                id_conta_origem AS IdContaOrigem,
                valor_estorno AS ValorEstorno,
                tentativas_realizadas AS TentativasRealizadas,
                data_criacao AS DataCriacao,
                data_ultima_retentativa AS DataUltimaRetentativa,
                data_resolucao AS DataResolucao,
                status AS Status,
                motivo_falha AS MotivoFalha,
                observacoes_operador AS ObservacoesOperador
            FROM compensacao_pendente
            WHERE status IN ('Pendente', 'Processando')
            ORDER BY data_criacao ASC
            LIMIT @Limite";

        using var connection = new SqliteConnection(_connectionString);
        var result = await connection.QueryAsync<CompensacaoPendente>(sql, new { Limite = limite });
        return result.ToList();
    }

    public async Task AdicionarAsync(CompensacaoPendente compensacao)
    {
        const string sql = @"
            INSERT INTO compensacao_pendente (
                id,
                id_transferencia,
                chave_idempotencia,
                id_conta_origem,
                valor_estorno,
                tentativas_realizadas,
                data_criacao,
                data_ultima_retentativa,
                data_resolucao,
                status,
                motivo_falha,
                observacoes_operador
            ) VALUES (
                @Id,
                @IdTransferencia,
                @ChaveIdempotencia,
                @IdContaOrigem,
                @ValorEstorno,
                @TentativasRealizadas,
                @DataCriacao,
                @DataUltimaRetentativa,
                @DataResolucao,
                @Status,
                @MotivoFalha,
                @ObservacoesOperador
            )";

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(sql, compensacao);
    }

    public async Task AtualizarAsync(CompensacaoPendente compensacao)
    {
        const string sql = @"
            UPDATE compensacao_pendente SET
                tentativas_realizadas = @TentativasRealizadas,
                data_ultima_retentativa = @DataUltimaRetentativa,
                data_resolucao = @DataResolucao,
                status = @Status,
                motivo_falha = @MotivoFalha,
                observacoes_operador = @ObservacoesOperador
            WHERE id = @Id";

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(sql, compensacao);
    }

    public async Task<int> ContarPendentesAsync()
    {
        const string sql = @"
            SELECT COUNT(*)
            FROM compensacao_pendente
            WHERE status IN ('Pendente', 'Processando')";

        using var connection = new SqliteConnection(_connectionString);
        return await connection.ExecuteScalarAsync<int>(sql);
    }
}
