using MediatR;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.Transferencia.Application.DTOs;
using BankMore.Transferencia.Application.Queries;

namespace BankMore.Transferencia.Application.Handlers;

public class ListarTransferenciasHandler : IRequestHandler<ListarTransferenciasQuery, PaginatedList<TransferenciaDto>>
{
    private readonly string _connectionString;

    public ListarTransferenciasHandler(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    public async Task<PaginatedList<TransferenciaDto>> Handle(ListarTransferenciasQuery request, CancellationToken cancellationToken)
    {
        using var connection = new SqliteConnection(_connectionString);

        // Construir WHERE clause baseado nos filtros
        var whereConditions = new List<string>();
        var parameters = new DynamicParameters();

        // Filtro por tipo (enviadas/recebidas/todas)
        if (request.Tipo == "enviadas")
        {
            whereConditions.Add("idcontacorrenteorigem = @IdContaCorrente");
        }
        else if (request.Tipo == "recebidas")
        {
            whereConditions.Add("idcontacorrentedestino = @IdContaCorrente");
        }
        else // todas
        {
            whereConditions.Add("(idcontacorrenteorigem = @IdContaCorrente OR idcontacorrentedestino = @IdContaCorrente)");
        }
        parameters.Add("IdContaCorrente", request.IdContaCorrente);

        // Filtro por data
        if (request.DataInicio.HasValue)
        {
            var dataInicioStr = request.DataInicio.Value.ToString("dd/MM/yyyy");
            whereConditions.Add("datatransferencia >= @DataInicio");
            parameters.Add("DataInicio", dataInicioStr);
        }

        if (request.DataFim.HasValue)
        {
            var dataFimStr = request.DataFim.Value.ToString("dd/MM/yyyy");
            whereConditions.Add("datatransferencia <= @DataFim");
            parameters.Add("DataFim", dataFimStr);
        }

        var whereClause = string.Join(" AND ", whereConditions);

        // Contar total
        var countSql = $"SELECT COUNT(*) FROM transferencia WHERE {whereClause}";
        var totalItems = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        // Buscar itens paginados
        var sql = $@"
            SELECT 
                idtransferencia AS Id,
                idcontacorrenteorigem AS IdContaCorrenteOrigem,
                idcontacorrentedestino AS IdContaCorrenteDestino,
                valor AS Valor,
                datatransferencia AS DataTransferencia
            FROM transferencia
            WHERE {whereClause}
            ORDER BY datatransferencia DESC
            LIMIT @PageSize OFFSET @Offset";

        parameters.Add("PageSize", request.PageSize);
        parameters.Add("Offset", (request.Page - 1) * request.PageSize);

        var items = (await connection.QueryAsync<TransferenciaDto>(sql, parameters)).ToList();

        // Determinar status de cada transferência
        foreach (var item in items)
        {
            item.Status = item.IdContaCorrenteOrigem == request.IdContaCorrente 
                ? "Enviada" 
                : "Recebida";
        }

        return new PaginatedList<TransferenciaDto>
        {
            Items = items,
            Page = request.Page,
            PageSize = request.PageSize,
            TotalItems = totalItems
        };
    }
}
