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
        Console.WriteLine($"[LOGIN HANDLER] Recebido: NumeroOuCpf='{request.NumeroOuCpf}', SenhaLength={request.Senha?.Length ?? 0}");
        
        Domain.Entities.ContaCorrente? conta = null;

        // Tenta buscar por número da conta
        if (int.TryParse(request.NumeroOuCpf, out int numeroConta))
        {
            Console.WriteLine($"[LOGIN HANDLER] Buscando por número da conta: {numeroConta}");
            conta = await _repository.ObterPorNumeroAsync(numeroConta);
            Console.WriteLine($"[LOGIN HANDLER] Conta encontrada por número? {conta != null}");
        }

        // Se não encontrou, tenta buscar por CPF (criptografa antes de buscar)
        if (conta == null)
        {
            Console.WriteLine($"[LOGIN HANDLER] Tentando buscar por CPF: {request.NumeroOuCpf}");
            var cpfCriptografado = _cryptographyService.Encrypt(request.NumeroOuCpf);
            conta = await _repository.ObterPorCpfAsync(cpfCriptografado);
            Console.WriteLine($"[LOGIN HANDLER] Conta encontrada por CPF? {conta != null}");
        }

        // Valida se a conta existe e se a senha está correta
        if (conta == null)
        {
            Console.WriteLine($"[LOGIN HANDLER] FALHA - Conta não encontrada");
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Credenciais inválidas");
        }
        
        var senhaValida = _cryptographyService.VerifyPassword(request.Senha ?? string.Empty, conta.Senha);
        Console.WriteLine($"[LOGIN HANDLER] Senha válida? {senhaValida}");
        
        if (!senhaValida)
        {
            Console.WriteLine($"[LOGIN HANDLER] FALHA - Senha incorreta");
            throw new UnauthorizedAccessException("USER_UNAUTHORIZED: Credenciais inválidas");
        }

        Console.WriteLine($"[LOGIN HANDLER] Login bem-sucedido! IdConta: {conta.IdContaCorrente}");

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
