using System.Security.Cryptography;
using System.Text;

namespace BankMore.ContaCorrente.Infrastructure.Services;

/// <summary>
/// Implementação do serviço de criptografia
/// Utiliza AES-256 para criptografia simétrica e BCrypt para senhas
/// </summary>
public class CryptographyService : ICryptographyService
{
    private readonly byte[] _key;
    private readonly byte[] _iv;

    public CryptographyService(string encryptionKey)
    {
        if (string.IsNullOrEmpty(encryptionKey))
            throw new ArgumentNullException(nameof(encryptionKey));

        if (encryptionKey.Length < 32)
            throw new ArgumentException("Chave de criptografia deve ter no mínimo 32 caracteres", nameof(encryptionKey));

        // Gerar chave de 256 bits (32 bytes) a partir da string
        using var sha256 = SHA256.Create();
        _key = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
        
        // Gerar IV de 128 bits (16 bytes) - fixo para permitir busca por CPF criptografado
        // Em produção, considere usar IV aleatório por registro se busca não for necessária
        _iv = sha256.ComputeHash(Encoding.UTF8.GetBytes(encryptionKey + "IV")).Take(16).ToArray();
    }

    /// <summary>
    /// Criptografa texto usando AES-256-CBC
    /// </summary>
    public string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return plainText;

        using var aes = Aes.Create();
        aes.Key = _key;
        aes.IV = _iv;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        
        return Convert.ToBase64String(cipherBytes);
    }

    /// <summary>
    /// Descriptografa texto criptografado com AES-256-CBC
    /// </summary>
    public string Decrypt(string cipherText)
    {
        if (string.IsNullOrEmpty(cipherText))
            return cipherText;

        try
        {
            using var aes = Aes.Create();
            aes.Key = _key;
            aes.IV = _iv;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;

            using var decryptor = aes.CreateDecryptor();
            var cipherBytes = Convert.FromBase64String(cipherText);
            var plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Erro ao descriptografar dados", ex);
        }
    }

    /// <summary>
    /// Cria hash SHA-256 irreversível
    /// </summary>
    public string Hash(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(text);
        var hash = sha256.ComputeHash(bytes);
        
        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// Gera hash de senha usando BCrypt com salt de 12 rounds
    /// </summary>
    public string HashPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentNullException(nameof(password));

        var salt = BCrypt.Net.BCrypt.GenerateSalt(12);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    /// <summary>
    /// Verifica se senha corresponde ao hash BCrypt
    /// </summary>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hash))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }
}
