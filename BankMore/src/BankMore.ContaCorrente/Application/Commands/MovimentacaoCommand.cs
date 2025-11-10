using MediatR;

namespace BankMore.ContaCorrente.Application.Commands;

/// <summary>
/// Command para realizar movimentação na conta corrente
/// </summary>
public class MovimentacaoCommand : IRequest<bool>
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string IdContaCorrente { get; set; } = string.Empty; // Do token JWT
    public int? NumeroConta { get; set; } // Opcional
    public char TipoMovimento { get; set; } // 'C' ou 'D'
    public decimal Valor { get; set; }
}
