using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Services;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Services;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar o comando de cadastro de conta corrente
/// Padrão MediatR (usado na Ailos)
/// </summary>
public class CadastrarContaHandler : IRequestHandler<CadastrarContaCommand, ContaDto>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICryptographyService _cryptographyService;

    public CadastrarContaHandler(
        IContaCorrenteRepository repository,
        IUnitOfWork unitOfWork,
        ICryptographyService cryptographyService)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _cryptographyService = cryptographyService ?? throw new ArgumentNullException(nameof(cryptographyService));
    }

    public async Task<ContaDto> Handle(CadastrarContaCommand request, CancellationToken cancellationToken)
    {
        // Validar CPF
        if (!CpfValidator.Validar(request.Cpf))
        {
            throw new InvalidOperationException("INVALID_DOCUMENT: CPF inválido");
        }

        // Criptografar CPF para armazenamento seguro
        var cpfCriptografado = _cryptographyService.Encrypt(request.Cpf);

        // Gerar hash da senha usando BCrypt através do serviço de criptografia
        var senhaHash = _cryptographyService.HashPassword(request.Senha);

        // Criar a entidade de domínio
        var novaConta = new Domain.Entities.ContaCorrente(cpfCriptografado, request.Nome, senhaHash, string.Empty);

        // Obter próximo número de conta
        var proximoNumero = await _repository.ObterProximoNumeroContaAsync();
        novaConta.DefinirNumero(proximoNumero);

        // Persistir no banco
        await _repository.AdicionarAsync(novaConta);
        await _unitOfWork.CommitAsync();

        return new ContaDto
        {
            Id = novaConta.IdContaCorrente,
            NumeroContaCorrente = novaConta.Numero,
            Cpf = request.Cpf, // Retorna CPF não criptografado no response
            Nome = novaConta.Nome,
            Ativo = novaConta.Ativo,
            Saldo = 0.00m,
            DataCriacao = DateTime.UtcNow
        };
    }
}
