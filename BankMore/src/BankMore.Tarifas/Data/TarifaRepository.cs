using Dapper;
using Microsoft.Data.Sqlite;
using BankMore.Tarifas.Models;

namespace BankMore.Tarifas.Data;

/// <summary>
/// Repositório para persistência de tarifas usando Dapper
/// </summary>
public interface ITarifaRepository
{
    Task<string> AdicionarAsync(Tarifa tarifa);
    Task<bool> JaProcessadaAsync(string idTransferencia);
}

public class TarifaRepository : ITarifaRepository
{
    private readonly string _connectionString;

    public TarifaRepository(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public async Task<string> AdicionarAsync(Tarifa tarifa)
    {
        const string sql = @"
            INSERT INTO tarifa (
                id,
                idcontacorrente,
                idtransferencia,
                valor,
                datamovimento
            ) VALUES (
                @Id,
                @IdContaCorrente,
                @IdTransferencia,
                @Valor,
                @DataMovimento
            )";

        using var connection = new SqliteConnection(_connectionString);
        await connection.ExecuteAsync(sql, tarifa);
        return tarifa.Id;
    }

    public async Task<bool> JaProcessadaAsync(string idTransferencia)
    {
        const string sql = "SELECT COUNT(1) FROM tarifa WHERE idtransferencia = @IdTransferencia";

        using var connection = new SqliteConnection(_connectionString);
        var count = await connection.ExecuteScalarAsync<int>(sql, new { IdTransferencia = idTransferencia });
        return count > 0;
    }
}
