using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Queries;

/// <summary>
/// Query para consultar o saldo da conta corrente
/// </summary>
public class ConsultarSaldoQuery : IRequest<SaldoDto>
{
    public string IdContaCorrente { get; set; } = string.Empty;
}
