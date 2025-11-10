using Xunit;
using FluentAssertions;
using BankMore.ContaCorrente.Application.Services;
using System.IdentityModel.Tokens.Jwt;

namespace BankMore.ContaCorrente.Tests.Services;

public class JwtServiceTests
{
    private readonly JwtService _jwtService;

    public JwtServiceTests()
    {
        // Configuração de teste
        _jwtService = new JwtService(
            secretKey: "BankMore-Super-Secret-JWT-Key-With-Minimum-32-Characters-For-Security!",
            issuer: "BankMoreTests",
            audience: "BankMoreTests",
            accessTokenExpiracaoMinutos: 10,
            refreshTokenExpiracaoDias: 1
        );
    }

    [Fact]
    public void GerarAccessToken_DeveRetornarTokenValido()
    {
        // Arrange
        var idConta = Guid.NewGuid().ToString();

        // Act
        var token = _jwtService.GerarAccessToken(idConta);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().Contain("."); // JWT tem 3 partes separadas por ponto
        token.Split('.').Should().HaveCount(3);
    }

    [Fact]
    public void GerarAccessToken_DeveConterClaimsObrigatorias()
    {
        // Arrange
        var idConta = Guid.NewGuid().ToString();
        var handler = new JwtSecurityTokenHandler();

        // Act
        var token = _jwtService.GerarAccessToken(idConta);
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == idConta);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Iat);
        jwtToken.Claims.Should().Contain(c => c.Type == "tipo" && c.Value == "access");
        
        jwtToken.Issuer.Should().Be("BankMoreTests");
        jwtToken.Audiences.Should().Contain("BankMoreTests");
    }

    [Fact]
    public void GerarAccessToken_NaoDeveConterDadosSensiveis()
    {
        // Arrange
        var idConta = Guid.NewGuid().ToString();
        var handler = new JwtSecurityTokenHandler();

        // Act
        var token = _jwtService.GerarAccessToken(idConta);
        var jwtToken = handler.ReadJwtToken(token);

        // Assert - Verificar que NÃO contém dados sensíveis
        jwtToken.Claims.Should().NotContain(c => c.Type == "cpf");
        jwtToken.Claims.Should().NotContain(c => c.Type == "nome");
        jwtToken.Claims.Should().NotContain(c => c.Type == "numero");
        jwtToken.Claims.Should().NotContain(c => c.Type == "saldo");
        jwtToken.Claims.Should().NotContain(c => c.Type == "email");
    }

    [Fact]
    public void GerarAccessToken_DeveExpirarEm10Minutos()
    {
        // Arrange
        var idConta = Guid.NewGuid().ToString();
        var handler = new JwtSecurityTokenHandler();

        // Act
        var token = _jwtService.GerarAccessToken(idConta);
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        var expiracao = jwtToken.ValidTo;
        var agora = DateTime.UtcNow;
        var diferenca = expiracao - agora;

        diferenca.TotalMinutes.Should().BeApproximately(10, 1); // ~10 minutos (±1 min tolerância)
    }

    [Fact]
    public void GerarRefreshToken_DeveRetornarTokenUnico()
    {
        // Arrange & Act
        var token1 = _jwtService.GerarRefreshToken();
        var token2 = _jwtService.GerarRefreshToken();

        // Assert
        token1.Should().NotBeNullOrEmpty();
        token2.Should().NotBeNullOrEmpty();
        token1.Should().NotBe(token2); // Tokens devem ser únicos
        
        // Refresh token deve ter tamanho adequado (base64 de 32 bytes = 44 chars)
        token1.Length.Should().BeGreaterThan(30);
    }

    [Fact]
    public void ValidarToken_ComTokenValido_DeveRetornarPrincipal()
    {
        // Arrange
        var idConta = Guid.NewGuid().ToString();
        var token = _jwtService.GerarAccessToken(idConta);

        // Act
        var principal = _jwtService.ValidarToken(token);

        // Assert
        principal.Should().NotBeNull();
        // O claim "sub" pode aparecer como "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        principal!.Claims.Should().Contain(c => 
            (c.Type == JwtRegisteredClaimNames.Sub || c.Type == System.Security.Claims.ClaimTypes.NameIdentifier) 
            && c.Value == idConta);
    }

    [Fact]
    public void ValidarToken_ComTokenInvalido_DeveRetornarNull()
    {
        // Arrange
        var tokenInvalido = "token.invalido.fake";

        // Act
        var principal = _jwtService.ValidarToken(tokenInvalido);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidarToken_ComTokenVazio_DeveRetornarNull()
    {
        // Arrange
        var tokenVazio = "";

        // Act
        var principal = _jwtService.ValidarToken(tokenVazio);

        // Assert
        principal.Should().BeNull();
    }

    [Theory]
    [InlineData("550e8400-e29b-41d4-a716-446655440000")]
    [InlineData("7c9e6679-7425-40de-944b-e07fc1f90ae7")]
    [InlineData("f47ac10b-58cc-4372-a567-0e02b2c3d479")]
    public void GerarAccessToken_ComDiferentesIds_DeveFuncionar(string idConta)
    {
        // Act
        var token = _jwtService.GerarAccessToken(idConta);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        token.Should().NotBeNullOrEmpty();
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == idConta);
    }

    [Fact]
    public void ComputarHashToken_DeveGerarHashSHA256()
    {
        // Arrange
        var refreshToken = _jwtService.GerarRefreshToken();

        // Act
        var hash = _jwtService.ComputarHashToken(refreshToken);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Length.Should().Be(64); // SHA256 gera 64 caracteres hexadecimais
        hash.Should().MatchRegex("^[a-f0-9]{64}$"); // Apenas hex minúsculos
    }

    [Fact]
    public void ComputarHashToken_MesmoToken_DeveMesmoHash()
    {
        // Arrange
        var refreshToken = "token-teste-fixo";

        // Act
        var hash1 = _jwtService.ComputarHashToken(refreshToken);
        var hash2 = _jwtService.ComputarHashToken(refreshToken);

        // Assert
        hash1.Should().Be(hash2); // Hash deve ser determinístico
    }

    [Fact]
    public void ComputarHashToken_TokensDiferentes_DeveHashesDiferentes()
    {
        // Arrange
        var token1 = _jwtService.GerarRefreshToken();
        var token2 = _jwtService.GerarRefreshToken();

        // Act
        var hash1 = _jwtService.ComputarHashToken(token1);
        var hash2 = _jwtService.ComputarHashToken(token2);

        // Assert
        hash1.Should().NotBe(hash2);
    }
}
