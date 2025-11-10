namespace BankMore.ContaCorrente.Infrastructure.Services;

/// <summary>
/// Interface para serviços de criptografia de dados sensíveis
/// Centraliza métodos de criptografia interna e externa
/// </summary>
public interface ICryptographyService
{
    /// <summary>
    /// Criptografa um texto usando AES-256
    /// </summary>
    /// <param name="plainText">Texto em texto claro</param>
    /// <returns>Texto criptografado em Base64</returns>
    string Encrypt(string plainText);

    /// <summary>
    /// Descriptografa um texto criptografado usando AES-256
    /// </summary>
    /// <param name="cipherText">Texto criptografado em Base64</param>
    /// <returns>Texto em texto claro</returns>
    string Decrypt(string cipherText);

    /// <summary>
    /// Cria um hash irreversível usando SHA-256
    /// </summary>
    /// <param name="text">Texto para criar hash</param>
    /// <returns>Hash em hexadecimal</returns>
    string Hash(string text);

    /// <summary>
    /// Gera um hash de senha usando BCrypt
    /// </summary>
    /// <param name="password">Senha em texto claro</param>
    /// <returns>Hash BCrypt</returns>
    string HashPassword(string password);

    /// <summary>
    /// Verifica se uma senha corresponde ao hash BCrypt
    /// </summary>
    /// <param name="password">Senha em texto claro</param>
    /// <param name="hash">Hash BCrypt armazenado</param>
    /// <returns>True se a senha está correta</returns>
    bool VerifyPassword(string password, string hash);
}
