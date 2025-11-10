using Microsoft.Data.Sqlite;

namespace BankMore.Tarifas.Data;

/// <summary>
/// Inicializador do banco de dados SQLite com schema de tarifas
/// </summary>
public class DatabaseInitializer
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(string connectionString, ILogger<DatabaseInitializer> logger)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task InitializeAsync()
    {
        try
        {
            using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            // Criar tabela de tarifas
            const string createTarifaTable = @"
                CREATE TABLE IF NOT EXISTS tarifa (
                    id TEXT(37) PRIMARY KEY,
                    idcontacorrente TEXT(37) NOT NULL,
                    idtransferencia TEXT(37) NOT NULL,
                    valor REAL NOT NULL CHECK (valor >= 0),
                    datamovimento TEXT(25) NOT NULL
                )";

            using var command = new SqliteCommand(createTarifaTable, connection);
            await command.ExecuteNonQueryAsync();

            // Criar índice para busca por transferência (idempotência)
            const string createIndex = @"
                CREATE INDEX IF NOT EXISTS idx_tarifa_transferencia 
                ON tarifa(idtransferencia)";

            using var indexCommand = new SqliteCommand(createIndex, connection);
            await indexCommand.ExecuteNonQueryAsync();

            _logger.LogInformation("Banco de dados de tarifas inicializado com sucesso");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao inicializar banco de dados de tarifas");
            throw;
        }
    }
}
