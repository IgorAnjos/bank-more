namespace BankMore.ContaCorrente.Domain.Entities;

/// <summary>
/// Entidade de domínio que representa uma Conta Corrente
/// </summary>
public class ContaCorrente
{
    public string IdContaCorrente { get; private set; }
    public int Numero { get; private set; }
    public string Cpf { get; private set; } = string.Empty; // CPF criptografado
    public string Nome { get; private set; }
    public bool Ativo { get; private set; }
    public string Senha { get; private set; }
    public string Salt { get; private set; }
    
    // Navigation property
    public virtual ICollection<Movimento> Movimentos { get; private set; }

    // Construtor privado para EF Core
    private ContaCorrente() 
    {
        Movimentos = new List<Movimento>();
    }

    // Construtor para criação de nova conta
    public ContaCorrente(string cpf, string nome, string senha, string salt)
    {
        IdContaCorrente = Guid.CreateVersion7().ToString();
        Cpf = cpf ?? throw new ArgumentNullException(nameof(cpf));
        Nome = nome ?? throw new ArgumentNullException(nameof(nome));
        Senha = senha ?? throw new ArgumentNullException(nameof(senha));
        Salt = salt ?? throw new ArgumentNullException(nameof(salt));
        Ativo = true; // Conta ativa por padrão ao criar
        Movimentos = new List<Movimento>();
    }

    public void DefinirNumero(int numero)
    {
        if (numero <= 0)
            throw new ArgumentException("Número da conta deve ser maior que zero", nameof(numero));
        
        Numero = numero;
    }

    public void Inativar()
    {
        Ativo = false;
    }

    public void Ativar()
    {
        Ativo = true;
    }

    public bool ValidarSenha(string senha, string saltArmazenado)
    {
        return BCrypt.Net.BCrypt.Verify(senha, Senha);
    }
}
