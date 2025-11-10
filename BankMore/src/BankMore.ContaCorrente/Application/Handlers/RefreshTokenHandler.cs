using MediatR;
using BankMore.ContaCorrente.Application.Commands;
using BankMore.ContaCorrente.Application.DTOs;
using BankMore.ContaCorrente.Application.Services;
using BankMore.ContaCorrente.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace BankMore.ContaCorrente.Application.Handlers;

/// <summary>
/// Handler para processar a renovação de token via refresh token
/// </summary>
public class RefreshTokenHandler : IRequestHandler<RefreshTokenCommand, LoginResponseDto>
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IContaCorrenteRepository _contaRepository;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _configuration;

    public RefreshTokenHandler(
        IRefreshTokenRepository refreshTokenRepository,
        IContaCorrenteRepository contaRepository,
        IJwtService jwtService,
        IConfiguration configuration)
    {
        _refreshTokenRepository = refreshTokenRepository ?? throw new ArgumentNullException(nameof(refreshTokenRepository));
        _contaRepository = contaRepository ?? throw new ArgumentNullException(nameof(contaRepository));
        _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<LoginResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            throw new UnauthorizedAccessException("INVALID_REFRESH_TOKEN: Refresh token é obrigatório");
        }

        // Computa o hash do token fornecido
        var tokenHash = _jwtService.ComputarHashToken(request.RefreshToken);

        // Busca o refresh token no banco
        var refreshTokenEntity = await _refreshTokenRepository.ObterPorTokenHashAsync(tokenHash);

        if (refreshTokenEntity == null)
        {
            throw new UnauthorizedAccessException("INVALID_REFRESH_TOKEN: Token não encontrado");
        }

        // Valida se o token está válido (não revogado e não expirado)
        if (!refreshTokenEntity.EstaValido())
        {
            throw new UnauthorizedAccessException("INVALID_REFRESH_TOKEN: Token inválido, expirado ou revogado");
        }

        // Busca a conta associada ao refresh token
        var conta = await _contaRepository.ObterPorIdAsync(refreshTokenEntity.IdContaCorrente);
        if (conta == null)
        {
            throw new UnauthorizedAccessException("INVALID_REFRESH_TOKEN: Conta não encontrada");
        }

        // Gera um novo access token - APENAS com ID opaco
        var novoAccessToken = _jwtService.GerarAccessToken(conta.IdContaCorrente);

        // Gera um novo refresh token
        var novoRefreshToken = _jwtService.GerarRefreshToken();
        var novoRefreshTokenHash = _jwtService.ComputarHashToken(novoRefreshToken);

        // Revoga o refresh token antigo
        refreshTokenEntity.Revogar("Substituído por novo token");
        await _refreshTokenRepository.AtualizarAsync(refreshTokenEntity);

        // Salva o novo refresh token
        var diasValidade = int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "1");
        var novoRefreshTokenEntity = new Domain.Entities.RefreshToken(
            conta.IdContaCorrente,
            novoRefreshTokenHash,
            diasValidade
        );

        await _refreshTokenRepository.AdicionarAsync(novoRefreshTokenEntity);

        return new LoginResponseDto
        {
            AccessToken = novoAccessToken,
            RefreshToken = novoRefreshToken,
            IdContaCorrente = conta.IdContaCorrente,
            NumeroConta = conta.Numero,
            ExpiresInMinutes = int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "10")
        };
    }
}
