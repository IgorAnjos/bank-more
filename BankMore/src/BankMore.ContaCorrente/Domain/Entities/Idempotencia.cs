using BankMore.ContaCorrente.Domain.Enums;

namespace BankMore.ContaCorrente.Domain.Entities;

/// <summary>
/// Entidade para controle de idempotência das requisições
/// </summary>
public class Idempotencia
{
    public string ChaveIdempotencia { get; private set; }
    public string Requisicao { get; private set; }
    public string Resultado { get; private set; }
    public IdempotenciaStatus Status { get; private set; }
    public string? ResultadoHash { get; private set; }
    public string? Metadata { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime DataExpiracao { get; private set; }

    // Construtor privado para EF Core
    private Idempotencia() { }

    public Idempotencia(string chaveIdempotencia, string requisicao, string resultado)
    {
        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            throw new ArgumentNullException(nameof(chaveIdempotencia));

        ChaveIdempotencia = chaveIdempotencia;
        Requisicao = requisicao ?? string.Empty;
        Resultado = resultado ?? string.Empty;
        Status = IdempotenciaStatus.Pending;
        DataCriacao = DateTime.UtcNow;
        DataExpiracao = DataCriacao.AddHours(24);
    }

    public void AtualizarResultado(string resultado, IdempotenciaStatus status, string? resultadoHash = null, string? metadata = null)
    {
        Resultado = resultado ?? string.Empty;
        Status = status;
        ResultadoHash = resultadoHash;
        Metadata = metadata;
    }

    public bool Expirou() => DateTime.UtcNow > DataExpiracao;
}
