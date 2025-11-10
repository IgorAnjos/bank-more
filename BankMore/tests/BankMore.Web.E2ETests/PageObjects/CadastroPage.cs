using OpenQA.Selenium;
using BankMore.Web.E2ETests.Infrastructure;

namespace BankMore.Web.E2ETests.PageObjects;

/// <summary>
/// Page Object para a página de Cadastro
/// </summary>
public class CadastroPage
{
    private readonly IWebDriver _driver;
    private readonly SeleniumTestBase _testBase;

    // Locators
    private By CpfInput => By.Id("cpf");
    private By NomeInput => By.Id("nome");
    private By SenhaInput => By.Id("senha");
    private By CriarContaButton => By.CssSelector("button[type='submit']");
    private By FazerLoginButton => By.CssSelector("button.btn-outline-primary");
    private By ErrorAlert => By.CssSelector(".alert-danger");
    private By SuccessAlert => By.CssSelector(".alert-success");
    private By LoadingSpinner => By.CssSelector(".spinner-border");

    public CadastroPage(IWebDriver driver, SeleniumTestBase testBase)
    {
        _driver = driver;
        _testBase = testBase;
    }

    public void NavigateToCadastro()
    {
        _testBase.NavigateTo("/cadastro");
        _testBase.WaitForElement(CpfInput);
    }

    public void PreencherFormulario(string cpf, string nome, string senha)
    {
        _testBase.FillInput(CpfInput, cpf);
        _testBase.FillInput(NomeInput, nome);
        _testBase.FillInput(SenhaInput, senha);
    }

    public void ClicarCriarConta()
    {
        _testBase.ClickButton(CriarContaButton);
    }

    public void ClicarFazerLogin()
    {
        _testBase.ClickButton(FazerLoginButton);
    }

    public void AguardarLoadingDesaparecer()
    {
        _testBase.WaitForLoadingToFinish();
    }

    public bool ExibeMensagemErro()
    {
        return _testBase.ElementExists(ErrorAlert);
    }

    public bool ExibeMensagemSucesso()
    {
        return _testBase.ElementExists(SuccessAlert);
    }

    public string ObterMensagemErro()
    {
        return _testBase.WaitForElement(ErrorAlert).Text;
    }

    public string ObterMensagemSucesso()
    {
        return _testBase.WaitForElement(SuccessAlert).Text;
    }

    public string ObterNumeroConta()
    {
        var mensagem = ObterMensagemSucesso();
        // Extrai o número da conta da mensagem: "Conta criada com sucesso! Número da conta: 12345"
        var match = System.Text.RegularExpressions.Regex.Match(mensagem, @"Número da conta: (\d+)");
        return match.Success ? match.Groups[1].Value : string.Empty;
    }

    public bool BotaoEstaDesabilitado()
    {
        var button = _driver.FindElement(CriarContaButton);
        var disabled = button.GetDomProperty("disabled");
        return disabled != null && disabled != "false";
    }
}
