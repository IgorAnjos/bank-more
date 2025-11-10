using System.ComponentModel.DataAnnotations;

namespace BankMore.Transferencia.Application.DTOs;

/// <summary>
/// DTO para requisição de transferência
/// </summary>
public class TransferenciaRequestDto
{
    /// <summary>
    /// Chave única para idempotência
    /// </summary>
    [Required(ErrorMessage = "Chave de idempotência é obrigatória")]
    public string ChaveIdempotencia { get; set; } = string.Empty;

    /// <summary>
    /// Número da conta destino
    /// </summary>
    [Required(ErrorMessage = "Número da conta destino é obrigatório")]
    [Range(1, int.MaxValue, ErrorMessage = "Número da conta deve ser maior que zero")]
    public int NumeroContaDestino { get; set; }

    /// <summary>
    /// Valor da transferência
    /// </summary>
    [Required(ErrorMessage = "Valor é obrigatório")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
    public decimal Valor { get; set; }
}

/// <summary>
/// DTO para resposta de transferência
/// </summary>
public class TransferenciaResponseDto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
}
