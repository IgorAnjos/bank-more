namespace BankMore.Transferencia.Domain.Entities;

/// <summary>
/// Entidade para rastreamento de compensações (estornos) pendentes
/// Usado quando retry automático falha - exige processamento assíncrono
/// Compliance: Circular BACEN 3.682/2013 - auditoria obrigatória
/// </summary>
public class CompensacaoPendente
{
    public string Id { get; private set; }
    public string IdTransferencia { get; private set; }
    public string ChaveIdempotencia { get; private set; }
    public string IdContaOrigem { get; private set; }
    public decimal ValorEstorno { get; private set; }
    public int TentativasRealizadas { get; private set; }
    public DateTime DataCriacao { get; private set; }
    public DateTime? DataUltimaRetentativa { get; private set; }
    public DateTime? DataResolucao { get; private set; }
    public string Status { get; private set; } // Pendente, Processando, Resolvido, EscaladoManual
    public string? MotivoFalha { get; private set; }
    public string? ObservacoesOperador { get; private set; }

    // Construtor privado para EF Core
    private CompensacaoPendente() 
    {
        Id = string.Empty;
        IdTransferencia = string.Empty;
        ChaveIdempotencia = string.Empty;
        IdContaOrigem = string.Empty;
        Status = string.Empty;
    }

    public CompensacaoPendente(
        string idTransferencia,
        string chaveIdempotencia,
        string idContaOrigem,
        decimal valorEstorno,
        string motivoFalha)
    {
        Id = Guid.NewGuid().ToString();
        IdTransferencia = idTransferencia ?? throw new ArgumentNullException(nameof(idTransferencia));
        ChaveIdempotencia = chaveIdempotencia ?? throw new ArgumentNullException(nameof(chaveIdempotencia));
        IdContaOrigem = idContaOrigem ?? throw new ArgumentNullException(nameof(idContaOrigem));
        ValorEstorno = valorEstorno;
        TentativasRealizadas = 0;
        DataCriacao = DateTime.UtcNow;
        Status = "Pendente";
        MotivoFalha = motivoFalha;
    }

    public void RegistrarTentativa(string? novaMensagemErro = null)
    {
        TentativasRealizadas++;
        DataUltimaRetentativa = DateTime.UtcNow;
        
        if (!string.IsNullOrEmpty(novaMensagemErro))
        {
            MotivoFalha = $"{MotivoFalha}\n[Tentativa {TentativasRealizadas}] {novaMensagemErro}";
        }

        // Após 5 tentativas, escalar para manual
        if (TentativasRealizadas >= 5)
        {
            Status = "EscaladoManual";
        }
        else
        {
            Status = "Processando";
        }
    }

    public void MarcarComoResolvido(string? observacoes = null)
    {
        Status = "Resolvido";
        DataResolucao = DateTime.UtcNow;
        ObservacoesOperador = observacoes;
    }

    public void AdicionarObservacao(string observacao)
    {
        ObservacoesOperador = string.IsNullOrEmpty(ObservacoesOperador)
            ? observacao
            : $"{ObservacoesOperador}\n{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {observacao}";
    }

    public bool PodeRetentarAutomaticamente() => TentativasRealizadas < 5 && Status != "EscaladoManual" && Status != "Resolvido";
}
