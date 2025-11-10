namespace BankMore.ContaCorrente.Domain.Entities;

/// <summary>
/// Entidade para gerenciamento de Refresh Tokens
/// Permite renovação de sessão sem re-autenticação
/// </summary>
public class RefreshToken
{
    public string Id { get; private set; }
    public string IdContaCorrente { get; private set; }
    public string TokenHash { get; private set; } // SHA-256 hash do token
    public DateTime DataCriacao { get; private set; }
    public DateTime DataExpiracao { get; private set; }
    public bool Revogado { get; private set; }
    public DateTime? DataRevogacao { get; private set; }
    public string? MotivoRevogacao { get; private set; }

    // Construtor privado para EF Core
    private RefreshToken() 
    {
        Id = string.Empty;
        IdContaCorrente = string.Empty;
        TokenHash = string.Empty;
    }

    public RefreshToken(string idContaCorrente, string tokenHash, int diasValidade = 1)
    {
        if (string.IsNullOrWhiteSpace(idContaCorrente))
            throw new ArgumentNullException(nameof(idContaCorrente));
        
        if (string.IsNullOrWhiteSpace(tokenHash))
            throw new ArgumentNullException(nameof(tokenHash));

        Id = Guid.NewGuid().ToString();
        IdContaCorrente = idContaCorrente;
        TokenHash = tokenHash;
        DataCriacao = DateTime.UtcNow;
        DataExpiracao = DataCriacao.AddDays(diasValidade);
        Revogado = false;
    }

    public void Revogar(string motivo)
    {
        Revogado = true;
        DataRevogacao = DateTime.UtcNow;
        MotivoRevogacao = motivo;
    }

    public bool EstaValido() => !Revogado && DateTime.UtcNow < DataExpiracao;
}
