using OpenQA.Selenium;
using BankMore.Web.E2ETests.Infrastructure;

namespace BankMore.Web.E2ETests.PageObjects;

/// <summary>
/// Page Object para a página de Login
/// </summary>
public class LoginPage
{
    private readonly IWebDriver _driver;
    private readonly SeleniumTestBase _testBase;

    // Locators
    private By NumeroContaOuCpfInput => By.Id("numeroContaOuCpf");
    private By SenhaInput => By.Id("senha");
    private By EntrarButton => By.CssSelector("button[type='submit']");
    private By CriarContaButton => By.CssSelector("button.btn-outline-primary");
    private By ErrorAlert => By.CssSelector(".alert-danger");
    private By SuccessAlert => By.CssSelector(".alert-success");
    private By LoadingSpinner => By.CssSelector(".spinner-border");

    public LoginPage(IWebDriver driver, SeleniumTestBase testBase)
    {
        _driver = driver;
        _testBase = testBase;
    }

    public void NavigateToLogin()
    {
        _testBase.NavigateTo("/login");
        _testBase.WaitForElement(NumeroContaOuCpfInput);
    }

    public void PreencherCredenciais(string numeroContaOuCpf, string senha)
    {
        _testBase.FillInput(NumeroContaOuCpfInput, numeroContaOuCpf);
        _testBase.FillInput(SenhaInput, senha);
    }

    public void ClicarEntrar()
    {
        _testBase.ClickButton(EntrarButton);
    }

    public void ClicarCriarConta()
    {
        _testBase.ClickButton(CriarContaButton);
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

    public bool BotaoEstaDesabilitado()
    {
        var button = _driver.FindElement(EntrarButton);
        var disabled = button.GetDomProperty("disabled");
        return disabled != null && disabled != "false";
    }

    public string ObterUrlAtual()
    {
        return _driver.Url;
    }
}
