using MediatR;

namespace BankMore.ContaCorrente.Application.Commands;

/// <summary>
/// Command para inativar uma conta corrente
/// </summary>
public class InativarContaCommand : IRequest<bool>
{
    public string IdContaCorrente { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
