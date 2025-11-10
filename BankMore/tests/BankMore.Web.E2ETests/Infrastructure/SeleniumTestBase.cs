using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace BankMore.Web.E2ETests.Infrastructure;

/// <summary>
/// Classe base para testes E2E com Selenium WebDriver
/// </summary>
public abstract class SeleniumTestBase : IDisposable
{
    protected IWebDriver Driver { get; private set; }
    protected WebDriverWait Wait { get; private set; }
    protected string BaseUrl { get; private set; }

    protected SeleniumTestBase()
    {
        // Configurações do Chrome
        var options = new ChromeOptions();
        options.AddArgument("--headless"); // Executar sem interface gráfica
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-notifications");
        options.AddArgument("--ignore-certificate-errors");

        Driver = new ChromeDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        
        // URL base da aplicação (deve estar rodando no Docker)
        BaseUrl = Environment.GetEnvironmentVariable("WEB_BASE_URL") ?? "http://localhost:8080";
        
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        Driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);
    }

    /// <summary>
    /// Navega para uma URL relativa
    /// </summary>
    public void NavigateTo(string relativeUrl)
    {
        var url = $"{BaseUrl}{relativeUrl}";
        Driver.Navigate().GoToUrl(url);
    }

    /// <summary>
    /// Aguarda um elemento ficar visível
    /// </summary>
    public IWebElement WaitForElement(By by, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        return wait.Until(drv => 
        {
            var element = drv.FindElement(by);
            return element.Displayed ? element : null;
        })!;
    }

    /// <summary>
    /// Aguarda um elemento ser clicável
    /// </summary>
    public IWebElement WaitForClickable(By by, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        return wait.Until(drv =>
        {
            var element = drv.FindElement(by);
            return element.Displayed && element.Enabled ? element : null;
        })!;
    }

    /// <summary>
    /// Aguarda um elemento desaparecer
    /// </summary>
    public void WaitForElementToDisappear(By by, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(drv =>
        {
            try
            {
                var element = drv.FindElement(by);
                return !element.Displayed;
            }
            catch (NoSuchElementException)
            {
                return true;
            }
        });
    }

    /// <summary>
    /// Aguarda um texto aparecer na página
    /// </summary>
    public void WaitForText(string text, int timeoutSeconds = 10)
    {
        var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(timeoutSeconds));
        wait.Until(drv => drv.PageSource.Contains(text));
    }

    /// <summary>
    /// Aguarda o spinner de loading desaparecer
    /// </summary>
    public void WaitForLoadingToFinish(int timeoutSeconds = 10)
    {
        try
        {
            WaitForElementToDisappear(By.CssSelector(".spinner-border"), timeoutSeconds);
        }
        catch (WebDriverTimeoutException)
        {
            // Se não encontrou spinner, não tem problema
        }
    }

    /// <summary>
    /// Preenche um campo de input
    /// </summary>
    public void FillInput(By by, string value)
    {
        var element = WaitForElement(by);
        element.Clear();
        element.SendKeys(value);
    }

    /// <summary>
    /// Clica em um botão
    /// </summary>
    public void ClickButton(By by)
    {
        var button = WaitForClickable(by);
        button.Click();
    }

    /// <summary>
    /// Verifica se um elemento existe
    /// </summary>
    public bool ElementExists(By by)
    {
        try
        {
            Driver.FindElement(by);
            return true;
        }
        catch (NoSuchElementException)
        {
            return false;
        }
    }

    /// <summary>
    /// Tira um screenshot (útil para debug)
    /// </summary>
    public void TakeScreenshot(string fileName)
    {
        var screenshot = ((ITakesScreenshot)Driver).GetScreenshot();
        var path = Path.Combine(Directory.GetCurrentDirectory(), "Screenshots", $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        screenshot.SaveAsFile(path);
    }

    /// <summary>
    /// Limpa cookies e localStorage
    /// </summary>
    public void ClearBrowserData()
    {
        Driver.Manage().Cookies.DeleteAllCookies();
        ((IJavaScriptExecutor)Driver).ExecuteScript("localStorage.clear();");
        ((IJavaScriptExecutor)Driver).ExecuteScript("sessionStorage.clear();");
    }

    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}
