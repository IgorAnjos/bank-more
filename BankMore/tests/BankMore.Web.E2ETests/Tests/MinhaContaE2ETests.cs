using FluentAssertions;
using BankMore.Web.E2ETests.Infrastructure;
using BankMore.Web.E2ETests.PageObjects;

namespace BankMore.Web.E2ETests.Tests;

/// <summary>
/// Testes E2E para a página Minha Conta (após autenticação)
/// </summary>
public class MinhaContaE2ETests : SeleniumTestBase
{
    private CadastroPage _cadastroPage;
    private LoginPage _loginPage;
    private MinhaContaPage _minhaContaPage;

    public MinhaContaE2ETests()
    {
        _cadastroPage = new CadastroPage(Driver, this);
        _loginPage = new LoginPage(Driver, this);
        _minhaContaPage = new MinhaContaPage(Driver, this);
    }

    [Fact(DisplayName = "Deve redirecionar para login quando não autenticado")]
    public void DeveRedirecionarParaLoginQuandoNaoAutenticado()
    {
        // Arrange
        ClearBrowserData(); // Garante que não há sessão

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        Thread.Sleep(1000);

        // Assert
        _minhaContaPage.ExibeAlertaNaoAutenticado().Should().BeTrue();
    }

    [Fact(DisplayName = "Deve exibir dados da conta após login")]
    public void DeveExibirDadosDaContaAposLogin()
    {
        // Arrange - Cria conta e faz login
        var cpf = GerarCpfAleatorio();
        var nome = "João Silva Conta";
        var senha = "senha123";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        _minhaContaPage.ExibeDadosConta().Should().BeTrue();
    }

    [Fact(DisplayName = "Deve exibir número da conta correto")]
    public void DeveExibirNumeroContaCorreto()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Maria Silva Numero";
        var senha = "senha456";

        var numeroConta = CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        var numeroExibido = _minhaContaPage.ObterNumeroConta();
        numeroExibido.Should().Be(numeroConta);
    }

    [Fact(DisplayName = "Deve exibir nome do titular correto")]
    public void DeveExibirNomeCorreto()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Pedro Santos Nome";
        var senha = "senha789";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        var nomeExibido = _minhaContaPage.ObterNome();
        nomeExibido.Should().Contain(nome);
    }

    [Fact(DisplayName = "Deve exibir CPF do titular")]
    public void DeveExibirCpfCorreto()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Ana Costa CPF";
        var senha = "senha321";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        var cpfExibido = _minhaContaPage.ObterCpf();
        cpfExibido.Should().Contain(cpf.Substring(0, 3)); // Verifica se contém parte do CPF
    }

    [Fact(DisplayName = "Deve exibir conta como ativa")]
    public void DeveExibirContaComoAtiva()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Carlos Oliveira Status";
        var senha = "senha654";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        _minhaContaPage.ContaEstaAtiva().Should().BeTrue();
        _minhaContaPage.ObterStatus().Should().Contain("Ativa");
    }

    [Fact(DisplayName = "Deve exibir saldo da conta")]
    public void DeveExibirSaldoDaConta()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Lucia Ferreira Saldo";
        var senha = "senha987";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        var saldo = _minhaContaPage.ObterSaldo();
        saldo.Should().NotBeNullOrEmpty();
        saldo.Should().Contain("R$"); // Formato monetário brasileiro
    }

    [Fact(DisplayName = "Deve exibir saldo inicial zerado")]
    public void DeveExibirSaldoInicialZerado()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Roberto Lima Saldo Zero";
        var senha = "senha111";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        var saldo = _minhaContaPage.ObterSaldo();
        saldo.Should().Contain("R$ 0,00");
    }

    [Fact(DisplayName = "Deve exibir página completa com todos os cards")]
    public void DeveExibirPaginaCompletaComTodosOsCards()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Fernanda Souza Completo";
        var senha = "senha222";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();

        // Assert
        _minhaContaPage.ExibeDadosConta().Should().BeTrue();
        
        // Verifica que consegue obter todos os dados principais
        _minhaContaPage.ObterNome().Should().NotBeNullOrEmpty();
        _minhaContaPage.ObterCpf().Should().NotBeNullOrEmpty();
        _minhaContaPage.ObterStatus().Should().NotBeNullOrEmpty();
        _minhaContaPage.ObterSaldo().Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "Deve manter sessão ao navegar entre páginas")]
    public void DeveManterSessaoAoNavegarEntrePaginas()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Gustavo Martins Sessao";
        var senha = "senha333";

        CriarContaEFazerLogin(cpf, nome, senha);

        // Act - Navega para diferentes páginas
        _minhaContaPage.NavigateToMinhaConta();
        _minhaContaPage.AguardarCarregamento();
        _minhaContaPage.ExibeDadosConta().Should().BeTrue();

        NavigateTo("/transferencias");
        Thread.Sleep(1000);

        NavigateTo("/conta");
        _minhaContaPage.AguardarCarregamento();

        // Assert - Ainda deve estar autenticado
        _minhaContaPage.ExibeDadosConta().Should().BeTrue();
    }

    /// <summary>
    /// Helper para criar conta e fazer login
    /// </summary>
    private string CriarContaEFazerLogin(string cpf, string nome, string senha)
    {
        // Cria a conta
        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        
        var numeroConta = _cadastroPage.ObterNumeroConta();
        Thread.Sleep(2500); // Aguarda redirecionamento

        // Faz login
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(numeroConta, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();
        Thread.Sleep(1000);

        return numeroConta;
    }

    /// <summary>
    /// Gera um CPF válido aleatório para testes
    /// </summary>
    private string GerarCpfAleatorio()
    {
        var random = new Random();
        var cpf = new int[11];

        for (int i = 0; i < 9; i++)
        {
            cpf[i] = random.Next(0, 10);
        }

        int soma = 0;
        for (int i = 0; i < 9; i++)
        {
            soma += cpf[i] * (10 - i);
        }
        int resto = soma % 11;
        cpf[9] = resto < 2 ? 0 : 11 - resto;

        soma = 0;
        for (int i = 0; i < 10; i++)
        {
            soma += cpf[i] * (11 - i);
        }
        resto = soma % 11;
        cpf[10] = resto < 2 ? 0 : 11 - resto;

        return string.Join("", cpf);
    }
}
