using MediatR;
using BankMore.ContaCorrente.Application.Queries;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para validar existência e status de conta corrente
/// Endpoint público (sem autenticação) para uso de outros microsserviços
/// Não expõe dados sensíveis
/// </summary>
public class ValidarContaHandler : IRequestHandler<ValidarContaQuery, bool>
{
    private readonly IContaCorrenteRepository _repository;

    public ValidarContaHandler(IContaCorrenteRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<bool> Handle(ValidarContaQuery request, CancellationToken cancellationToken)
    {
        // Buscar conta apenas por número (não sensível)
        var conta = await _repository.ObterPorNumeroAsync(request.NumeroConta);

        // Retorna true apenas se conta existe E está ativa
        // Não expõe motivo da falha (segurança por obscuridade)
        return conta != null && conta.Ativo;
    }
}
