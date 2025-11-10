using OpenQA.Selenium;
using BankMore.Web.E2ETests.Infrastructure;

namespace BankMore.Web.E2ETests.PageObjects;

/// <summary>
/// Page Object para a página Minha Conta
/// </summary>
public class MinhaContaPage
{
    private readonly IWebDriver _driver;
    private readonly SeleniumTestBase _testBase;

    // Locators
    private By NumeroContaText => By.XPath("//strong[text()='Número da Conta:']/following-sibling::text()");
    private By NomeText => By.XPath("//strong[text()='Nome:']/parent::div");
    private By CpfText => By.XPath("//strong[text()='CPF:']/parent::div");
    private By StatusBadge => By.CssSelector(".badge");
    private By SaldoDisplay => By.CssSelector(".display-4");
    private By DataCriacaoText => By.XPath("//strong[text()='Data de Criação:']/parent::div");
    private By AlertWarning => By.CssSelector(".alert-warning");
    private By LoadingSpinner => By.CssSelector(".spinner-border");
    private By DadosContaCard => By.XPath("//h4[text()='Dados da Conta']");

    public MinhaContaPage(IWebDriver driver, SeleniumTestBase testBase)
    {
        _driver = driver;
        _testBase = testBase;
    }

    public void NavigateToMinhaConta()
    {
        _testBase.NavigateTo("/conta");
    }

    public void AguardarCarregamento()
    {
        _testBase.WaitForLoadingToFinish();
        _testBase.WaitForElement(DadosContaCard, 15);
    }

    public bool ExibeAlertaNaoAutenticado()
    {
        return _testBase.ElementExists(AlertWarning);
    }

    public bool ExibeDadosConta()
    {
        return _testBase.ElementExists(DadosContaCard);
    }

    public string ObterNumeroConta()
    {
        var element = _testBase.WaitForElement(NomeText); // Usa Nome como âncora
        var parent = element.FindElement(By.XPath("..")).FindElement(By.XPath("preceding-sibling::div"));
        return parent.Text.Replace("Número da Conta:", "").Trim();
    }

    public string ObterNome()
    {
        return _testBase.WaitForElement(NomeText).Text.Replace("Nome:", "").Trim();
    }

    public string ObterCpf()
    {
        return _testBase.WaitForElement(CpfText).Text.Replace("CPF:", "").Trim();
    }

    public string ObterStatus()
    {
        return _testBase.WaitForElement(StatusBadge).Text;
    }

    public string ObterSaldo()
    {
        return _testBase.WaitForElement(SaldoDisplay).Text;
    }

    public bool ContaEstaAtiva()
    {
        var badge = _testBase.WaitForElement(StatusBadge);
        var cssClass = badge.GetDomAttribute("class") ?? "";
        return badge.Text.Contains("Ativa") && cssClass.Contains("bg-success");
    }

    public string ObterUrlAtual()
    {
        return _driver.Url;
    }
}
