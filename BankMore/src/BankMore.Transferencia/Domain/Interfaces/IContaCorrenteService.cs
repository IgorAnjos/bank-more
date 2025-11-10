namespace BankMore.Transferencia.Domain.Interfaces;

/// <summary>
/// Interface para serviço de comunicação com API Conta Corrente
/// </summary>
public interface IContaCorrenteService
{
    /// <summary>
    /// Valida se uma conta existe e está ativa usando apenas o número da conta (não sensível)
    /// </summary>
    /// <param name="numeroConta">Número da conta corrente</param>
    /// <returns>True se a conta existe e está ativa</returns>
    Task<bool> ValidarContaExistenteAsync(int numeroConta);

    /// <summary>
    /// Realiza uma movimentação (crédito ou débito) na conta corrente
    /// </summary>
    Task<bool> RealizarMovimentacaoAsync(string token, string chaveIdempotencia, int? numeroConta, char tipoMovimento, decimal valor);
}
