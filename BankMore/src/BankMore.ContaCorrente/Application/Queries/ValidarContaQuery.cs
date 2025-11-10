using MediatR;

namespace BankMore.ContaCorrente.Application.Queries;

/// <summary>
/// Query para validar se uma conta existe e está ativa
/// Usado por outros microsserviços (ex: Transferência) para validação
/// Não expõe dados sensíveis - apenas retorna true/false
/// </summary>
public class ValidarContaQuery : IRequest<bool>
{
    public int NumeroConta { get; set; }
}
