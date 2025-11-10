using MediatR;
using BankMore.Transferencia.Application.DTOs;

namespace BankMore.Transferencia.Application.Queries;

/// <summary>
/// Query para obter detalhes de uma transferência específica
/// </summary>
public class ObterTransferenciaQuery : IRequest<TransferenciaDto?>
{
    public string IdTransferencia { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty;
}
