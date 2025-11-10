using BankMore.Transferencia.Domain.Enums;

namespace BankMore.Transferencia.Domain.Entities;

/// <summary>
/// Entidade para controle de idempotência das requisições
/// </summary>
public class Idempotencia
{
    public string ChaveIdempotencia { get; set; } = string.Empty;
    public string Requisicao { get; set; } = string.Empty;
    public string Resultado { get; set; } = string.Empty;
    public IdempotenciaStatus Status { get; set; }
    public string? ResultadoHash { get; set; }
    public string? Metadata { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime DataExpiracao { get; set; }

    public Idempotencia() { }

    public Idempotencia(string chaveIdempotencia, string requisicao, string resultado)
    {
        ChaveIdempotencia = chaveIdempotencia ?? throw new ArgumentNullException(nameof(chaveIdempotencia));
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
