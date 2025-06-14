using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace ES2_webapp.UITests;

public abstract class TestBase
{
    protected IWebDriver Driver;
    protected WebDriverWait Wait;
    protected const string BaseUrl = "https://localhost:5001"; // Ajusta para a URL do teu projeto

    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--disable-notifications");
        
        Driver = new ChromeDriver(options);
        Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
        
        Driver.Navigate().GoToUrl(BaseUrl);
    }

    [TearDown]
    public void TearDown()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }

    protected void WaitForElement(By by)
    {
        Wait.Until(driver => driver.FindElement(by).Displayed);
    }

    protected void WaitForElementToBeClickable(By by)
    {
        Wait.Until(driver => driver.FindElement(by).Enabled);
    }

    protected void WaitForElementToDisappear(By by)
    {
        Wait.Until(driver => !driver.FindElement(by).Displayed);
    }

    protected void Login(string username, string password)
    {
        WaitForElement(By.Id("username"));
        Driver.FindElement(By.Id("username")).SendKeys(username);
        Driver.FindElement(By.Id("password")).SendKeys(password);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
    }
} 