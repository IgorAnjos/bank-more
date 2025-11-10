using MediatR;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using BankMore.Transferencia.Application.DTOs;
using BankMore.Transferencia.Application.Queries;
using BankMore.Transferencia.Domain.Interfaces;

namespace BankMore.Transferencia.Application.Handlers;

public class ObterTransferenciaHandler : IRequestHandler<ObterTransferenciaQuery, TransferenciaDto?>
{
    private readonly ITransferenciaRepository _repository;
    private readonly string _connectionString;

    public ObterTransferenciaHandler(ITransferenciaRepository repository, IConfiguration configuration)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string não configurada");
    }

    public async Task<TransferenciaDto?> Handle(ObterTransferenciaQuery request, CancellationToken cancellationToken)
    {
        const string sql = @"
            SELECT 
                idtransferencia AS Id,
                idcontacorrenteorigem AS IdContaCorrenteOrigem,
                idcontacorrentedestino AS IdContaCorrenteDestino,
                valor AS Valor,
                datatransferencia AS DataTransferencia
            FROM transferencia
            WHERE idtransferencia = @IdTransferencia
              AND (idcontacorrenteorigem = @IdContaCorrente OR idcontacorrentedestino = @IdContaCorrente)";

        using var connection = new SqliteConnection(_connectionString);
        
        var transferencia = await connection.QueryFirstOrDefaultAsync<TransferenciaDto>(
            sql,
            new
            {
                IdTransferencia = request.IdTransferencia,
                IdContaCorrente = request.IdContaCorrente
            }
        );

        if (transferencia != null)
        {
            // Determinar se é enviada ou recebida
            transferencia.Status = transferencia.IdContaCorrenteOrigem == request.IdContaCorrente 
                ? "Enviada" 
                : "Recebida";
        }

        return transferencia;
    }
}
