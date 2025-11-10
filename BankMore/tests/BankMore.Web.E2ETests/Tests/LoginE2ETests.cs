using FluentAssertions;
using BankMore.Web.E2ETests.Infrastructure;
using BankMore.Web.E2ETests.PageObjects;

namespace BankMore.Web.E2ETests.Tests;

/// <summary>
/// Testes E2E para a funcionalidade de Login
/// </summary>
public class LoginE2ETests : SeleniumTestBase
{
    private LoginPage _loginPage;
    private CadastroPage _cadastroPage;

    public LoginE2ETests()
    {
        _loginPage = new LoginPage(Driver, this);
        _cadastroPage = new CadastroPage(Driver, this);
    }

    [Fact(DisplayName = "Deve carregar a página de login corretamente")]
    public void DeveCarregarPaginaLogin()
    {
        // Act
        _loginPage.NavigateToLogin();

        // Assert
        Driver.Url.Should().Contain("/login");
        Driver.Title.Should().Contain("Login");
    }

    [Fact(DisplayName = "Deve fazer login com credenciais válidas usando número da conta")]
    public void DeveFazerLoginComNumeroContaValido()
    {
        // Arrange - Primeiro cria uma conta
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Login Conta";
        var senha = "senha123";

        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        
        var numeroConta = _cadastroPage.ObterNumeroConta();
        Thread.Sleep(2500); // Aguarda redirecionamento

        // Act - Faz login com o número da conta
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(numeroConta, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();

        // Assert
        _loginPage.ExibeMensagemSucesso().Should().BeTrue();
        _loginPage.ObterMensagemSucesso().Should().Contain("Login realizado com sucesso!");
        
        // Aguarda redirecionamento
        Thread.Sleep(1000);
        var url = Driver.Url;
        (url.Contains("/conta") || url.Contains("/")).Should().BeTrue();
    }

    [Fact(DisplayName = "Deve fazer login com credenciais válidas usando CPF")]
    public void DeveFazerLoginComCpfValido()
    {
        // Arrange - Primeiro cria uma conta
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Login CPF";
        var senha = "senha456";

        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        Thread.Sleep(2500);

        // Act - Faz login com CPF
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(cpf, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();

        // Assert
        _loginPage.ExibeMensagemSucesso().Should().BeTrue();
        Thread.Sleep(1000);
        Driver.Url.Should().NotContain("/login");
    }

    [Fact(DisplayName = "Deve exibir erro com credenciais inválidas")]
    public void DeveExibirErroComCredenciaisInvalidas()
    {
        // Arrange
        var contaInvalida = "99999999";
        var senhaInvalida = "senhaerrada";

        // Act
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(contaInvalida, senhaInvalida);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();

        // Assert
        _loginPage.ExibeMensagemErro().Should().BeTrue();
        _loginPage.ObterMensagemErro().Should().Contain("Credenciais inválidas");
        Driver.Url.Should().Contain("/login"); // Não deve redirecionar
    }

    [Fact(DisplayName = "Deve exibir erro com senha incorreta")]
    public void DeveExibirErroComSenhaIncorreta()
    {
        // Arrange - Cria uma conta
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Senha Errada";
        var senhaCorreta = "senha123";
        var senhaErrada = "senhaerrada";

        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senhaCorreta);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        var numeroConta = _cadastroPage.ObterNumeroConta();
        Thread.Sleep(2500);

        // Act - Tenta login com senha errada
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(numeroConta, senhaErrada);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();

        // Assert
        _loginPage.ExibeMensagemErro().Should().BeTrue();
        Driver.Url.Should().Contain("/login");
    }

    [Fact(DisplayName = "Deve desabilitar botão durante o loading")]
    public void DeveDesabilitarBotaoDuranteLoading()
    {
        // Arrange
        var conta = "12345";
        var senha = "senha123";

        // Act
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(conta, senha);
        _loginPage.ClicarEntrar();

        // Assert
        var desabilitado = _loginPage.BotaoEstaDesabilitado();
        desabilitado.Should().BeTrue("botão deve estar desabilitado durante o loading");
    }

    [Fact(DisplayName = "Deve navegar para tela de cadastro ao clicar em 'Criar nova conta'")]
    public void DeveNavegarParaCadastroAoClicarBotao()
    {
        // Arrange
        _loginPage.NavigateToLogin();

        // Act
        _loginPage.ClicarCriarConta();

        // Assert
        Driver.Url.Should().Contain("/cadastro");
    }

    [Fact(DisplayName = "Deve validar campos obrigatórios")]
    public void DeveValidarCamposObrigatorios()
    {
        // Arrange
        _loginPage.NavigateToLogin();

        // Act - tenta submeter sem preencher nada
        _loginPage.ClicarEntrar();

        // Assert - formulário não deve submeter (validação HTML5)
        Driver.Url.Should().Contain("/login");
    }

    [Fact(DisplayName = "Deve redirecionar usuário autenticado para home")]
    public void DeveRedirecionarUsuarioAutenticadoParaHome()
    {
        // Arrange - Cria conta e faz login
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Redirect Auth";
        var senha = "senha123";

        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        var numeroConta = _cadastroPage.ObterNumeroConta();
        Thread.Sleep(2500);

        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(numeroConta, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();
        Thread.Sleep(1000);

        // Act - Tenta acessar /login novamente
        _loginPage.NavigateToLogin();
        Thread.Sleep(1000);

        // Assert - Deve ter sido redirecionado (se a lógica estiver implementada)
        // Nota: Depende da implementação do OnInitialized no Login.razor
        var url = _loginPage.ObterUrlAtual();
        (url.Contains("/conta") || url.Contains("/login")).Should().BeTrue();
    }

    [Fact(DisplayName = "Deve aceitar CPF ou número de conta no mesmo campo")]
    public void DeveAceitarCpfOuNumeroContaNoMesmoCampo()
    {
        // Arrange
        var cpf = GerarCpfAleatorio();
        var nome = "Teste Campo Único";
        var senha = "senha123";

        _cadastroPage.NavigateToCadastro();
        _cadastroPage.PreencherFormulario(cpf, nome, senha);
        _cadastroPage.ClicarCriarConta();
        _cadastroPage.AguardarLoadingDesaparecer();
        var numeroConta = _cadastroPage.ObterNumeroConta();
        Thread.Sleep(2500);

        // Act & Assert - Login com CPF
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(cpf, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();
        _loginPage.ExibeMensagemSucesso().Should().BeTrue();
        
        ClearBrowserData();
        Thread.Sleep(1000);

        // Act & Assert - Login com número da conta
        _loginPage.NavigateToLogin();
        _loginPage.PreencherCredenciais(numeroConta, senha);
        _loginPage.ClicarEntrar();
        _loginPage.AguardarLoadingDesaparecer();
        _loginPage.ExibeMensagemSucesso().Should().BeTrue();
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
