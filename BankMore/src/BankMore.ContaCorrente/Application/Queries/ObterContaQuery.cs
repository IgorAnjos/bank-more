using MediatR;
using BankMore.ContaCorrente.Application.DTOs;

namespace BankMore.ContaCorrente.Application.Queries;

/// <summary>
/// Query para obter detalhes de uma conta específica
/// </summary>
public class ObterContaQuery : IRequest<ContaDto?>
{
    public string IdContaCorrente { get; set; } = string.Empty;
}
