using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace BankMore.ContaCorrente.Application.Services;

/// <summary>
/// Serviço para geração e validação de tokens JWT (access e refresh)
/// </summary>
public interface IJwtService
{
    string GerarAccessToken(string idContaCorrente);
    string GerarRefreshToken();
    string ComputarHashToken(string token);
    ClaimsPrincipal? ValidarToken(string token);
}

public class JwtService : IJwtService
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpiracaoMinutos;
    private readonly int _refreshTokenExpiracaoDias;

    public JwtService(
        string secretKey, 
        string issuer, 
        string audience, 
        int accessTokenExpiracaoMinutos = 10,
        int refreshTokenExpiracaoDias = 1)
    {
        _secretKey = secretKey ?? throw new ArgumentNullException(nameof(secretKey));
        _issuer = issuer;
        _audience = audience;
        _accessTokenExpiracaoMinutos = accessTokenExpiracaoMinutos;
        _refreshTokenExpiracaoDias = refreshTokenExpiracaoDias;
    }

    public string GerarAccessToken(string idContaCorrente)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, idContaCorrente),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
            new Claim("tipo", "access")
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiracaoMinutos),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GerarRefreshToken()
    {
        // Gera token criptograficamente seguro (32 bytes = 256 bits)
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        return Convert.ToBase64String(randomBytes);
    }

    public string ComputarHashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    public ClaimsPrincipal? ValidarToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                IssuerSigningKey = securityKey,
                ClockSkew = TimeSpan.Zero // Remove tolerância de 5min padrão
            }, out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}

