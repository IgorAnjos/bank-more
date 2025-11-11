using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Queries;

/// <summary>
/// Query para buscar dados da conta pelo número
/// Usado por outros microsserviços (ex: Transferência)
/// </summary>
public class ObterContaPorNumeroQuery : IRequest<ContaDto?>
{
    public int NumeroConta { get; set; }
}
