using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Domain.Interfaces;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar o comando de inativar conta corrente
/// </summary>
public class InativarContaHandler : IRequestHandler<InativarContaCommand, bool>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public InativarContaHandler(
        IContaCorrenteRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<bool> Handle(InativarContaCommand request, CancellationToken cancellationToken)
    {
        // Buscar a conta
        var conta = await _repository.ObterPorIdAsync(request.IdContaCorrente);

        if (conta == null)
        {
            throw new InvalidOperationException("INVALID_ACCOUNT: Conta não encontrada");
        }

        // Validar senha
        if (!conta.ValidarSenha(request.Senha, conta.Salt))
        {
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Senha incorreta");
        }

        // Inativar a conta
        conta.Inativar();

        // Persistir
        await _repository.AtualizarAsync(conta);
        await _unitOfWork.CommitAsync();

        return true;
    }
}
