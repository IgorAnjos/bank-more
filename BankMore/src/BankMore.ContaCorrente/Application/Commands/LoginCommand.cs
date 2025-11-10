using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Commands;

/// <summary>
/// Command para efetuar login
/// </summary>
public class LoginCommand : IRequest<LoginResponseDto>
{
    public string NumeroOuCpf { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}
