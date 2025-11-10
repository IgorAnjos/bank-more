using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Services;
using BankMore.ContaCorrente.Domain.Interfaces;
using BankMore.ContaCorrente.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar o comando de login
/// </summary>
public class LoginHandler : IRequestHandler<LoginCommand, LoginResponseDto>
{
    private readonly IContaCorrenteRepository _repository;
    private readonly IJwtService _jwtService;
    private readonly ICryptographyService _cryptographyService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IConfiguration _configuration;

    public LoginHandler(
        IContaCorrenteRepository repository,
        IJwtService jwtService,
        ICryptographyService cryptographyService,
        IRefreshTokenRepository refreshTokenRepository,
        IConfiguration configuration)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _cryptographyService = cryptographyService ?? throw new ArgumentNullException(nameof(cryptographyService));
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<LoginResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        Domain.Entities.ContaCorrente? conta = null;

        // Tenta buscar por número da conta
        if (int.TryParse(request.NumeroOuCpf, out int numeroConta))
        {
            conta = await _repository.ObterPorNumeroAsync(numeroConta);
        }

        // Se não encontrou, tenta buscar por CPF (criptografa antes de buscar)
        if (conta == null)
        {
            var cpfCriptografado = _cryptographyService.Encrypt(request.NumeroOuCpf);
            conta = await _repository.ObterPorCpfAsync(cpfCriptografado);
        }

        // Valida se a conta existe e se a senha está correta
        if (conta == null || !_cryptographyService.VerifyPassword(request.Senha, conta.Senha))
        {
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Credenciais inválidas");
        }

        // Gera o access token (curta duração) - APENAS com ID opaco
        var accessToken = _jwtService.GerarAccessToken(conta.IdContaCorrente);

        // Gera o refresh token (longa duração)
        var refreshToken = _jwtService.GerarRefreshToken();
        var refreshTokenHash = _jwtService.ComputarHashToken(refreshToken);

        // Salva o refresh token no banco
        var diasValidade = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "1");
        var refreshTokenEntity = new Domain.Entities.RefreshToken(
            conta.IdContaCorrente,
            refreshTokenHash,
            diasValidade
        );

        await _refreshTokenRepository.AdicionarAsync(refreshTokenEntity);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            IdContaCorrente = conta.IdContaCorrente,
            NumeroConta = conta.Numero,
            ExpiresInMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "10")
        };
    }
}
