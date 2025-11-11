using MediatR;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.Transferencia.Application.DTOs;
using BankMore.Transferencia.Application.Queries;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Application.Handlers;

public class ListarTransferenciasHandler : IRequestHandler<ListarTransferenciasQuery, PaginatedList<TransferenciaDto>>
{
    private readonly string _connectionString;
    private readonly IContaCorrenteService _contaCorrenteService;

    public ListarTransferenciasHandler(IConfiguration configuration, IContaCorrenteService contaCorrenteService)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string não configurada");
        _contaCorrenteService = contaCorrenteService ?? throw new ArgumentNullException(nameof(contaCorrenteService));
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
            whereConditions.Add("idcontacorrente_origem = @IdContaCorrente");
        }
        else if (request.Tipo == "recebidas")
        {
            whereConditions.Add("idcontacorrente_destino = @IdContaCorrente");
        }
        else // todas
        {
            whereConditions.Add("(idcontacorrente_origem = @IdContaCorrente OR idcontacorrente_destino = @IdContaCorrente)");
        }
        parameters.Add("IdContaCorrente", request.IdContaCorrente);

        // Filtro por data
        if (request.DataInicio.HasValue)
        {
            var dataInicioStr = request.DataInicio.Value.ToString("dd/MM/yyyy");
            whereConditions.Add("datamovimento >= @DataInicio");
            parameters.Add("DataInicio", dataInicioStr);
        }

        if (request.DataFim.HasValue)
        {
            var dataFimStr = request.DataFim.Value.ToString("dd/MM/yyyy");
            whereConditions.Add("datamovimento <= @DataFim");
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
                idcontacorrente_origem AS IdContaCorrenteOrigem,
                idcontacorrente_destino AS IdContaCorrenteDestino,
                valor AS Valor,
                datamovimento AS DataTransferencia
            FROM transferencia
            WHERE {whereClause}
            ORDER BY datamovimento DESC
            LIMIT @PageSize OFFSET @Offset";

        parameters.Add("PageSize", request.PageSize);
        parameters.Add("Offset", (request.Page - 1) * request.PageSize);

        var items = (await connection.QueryAsync<TransferenciaDto>(sql, parameters)).ToList();

        // Popula campos para evitar erro de deserialização no frontend
        // TODO: Implementar cache ou buscar números das contas em batch
        foreach (var item in items)
        {
            // Por enquanto deixamos null/0 para evitar N+1 queries
            // O frontend deve tratar estes casos
            item.NumeroContaOrigem = null;
            item.NumeroContaDestino = 0;

            // Determinar status
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
