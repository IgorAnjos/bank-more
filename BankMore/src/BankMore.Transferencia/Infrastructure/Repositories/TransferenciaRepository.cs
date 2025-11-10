using System.Data;
using Microsoft.Data.Sqlite;
using Dapper;
using BankMore.Transferencia.Domain.Entities;
using BankMore.Transferencia.Domain.Interfaces;
using TransferenciaEntity = BankMore.Transferencia.Domain.Entities.Transferencia;

namespace BankMore.Transferencia.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Transferência usando Dapper (padrão Ailos)
/// Raw SQL queries para máximo controle
/// </summary>
public class TransferenciaRepository : ITransferenciaRepository
{
    private readonly string _connectionString;

    public TransferenciaRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<TransferenciaEntity?> ObterPorIdAsync(string id)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                idcontacorrenteorigem AS IdContaCorrenteOrigem,
                idcontacorrentedestino AS IdContaCorrenteDestino,
                datamovimento AS DataMovimento,
                valor AS Valor
            FROM transferencia
            WHERE id = @Id";

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryFirstOrDefaultAsync<TransferenciaEntity>(sql, new { Id = id });
    }

    public async Task<string> AdicionarAsync(TransferenciaEntity transferencia)
    {
        const string sql = @"
            INSERT INTO transferencia (
                id,
                idcontacorrenteorigem,
                idcontacorrentedestino,
                datamovimento,
                valor
            ) VALUES (
                @Id,
                @IdContaCorrenteOrigem,
                @IdContaCorrenteDestino,
                @DataMovimento,
                @Valor
            )";

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(sql, transferencia);
        return transferencia.Id;
    }

    public async Task<IEnumerable<TransferenciaEntity>> ObterPorContaOrigemAsync(string idContaCorrenteOrigem)
    {
        const string sql = @"
            SELECT 
                id AS Id,
                idcontacorrenteorigem AS IdContaCorrenteOrigem,
                idcontacorrentedestino AS IdContaCorrenteDestino,
                datamovimento AS DataMovimento,
                valor AS Valor
            FROM transferencia
            WHERE idcontacorrenteorigem = @IdContaCorrenteOrigem
            ORDER BY datamovimento DESC";

        using var connection = new SqliteConnection(_connectionString);
        return await connection.QueryAsync<TransferenciaEntity>(sql, new { IdContaCorrenteOrigem = idContaCorrenteOrigem });
    }
}
