using MediatR;
using BankMore.Transferencia.Application.DTOs;

namespace BankMore.Transferencia.Application.Commands;

/// <summary>
/// Command para realizar transferência entre contas
/// </summary>
public class RealizarTransferenciaCommand : IRequest<TransferenciaDto>
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string IdContaCorrenteOrigem { get; set; } = string.Empty; // Do token JWT
    public int NumeroContaDestino { get; set; }
    public decimal Valor { get; set; }
    public string Token { get; set; } = string.Empty; // Para repassar à API Conta
}
