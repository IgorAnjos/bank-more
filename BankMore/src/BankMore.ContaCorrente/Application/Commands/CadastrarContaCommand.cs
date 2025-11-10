using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Commands;

/// <summary>
/// Comando para cadastrar uma nova conta corrente
/// </summary>
public class CadastrarContaCommand : IRequest<ContaDto>
{
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
