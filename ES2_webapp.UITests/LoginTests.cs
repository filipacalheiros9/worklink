using NUnit.Framework;
using OpenQA.Selenium;

namespace ES2_webapp.UITests;

[TestFixture]
public class LoginTests : TestBase
{
    [Test]
    public void Login_WithValidCredentials_ShouldNavigateToDashboard()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";

        // Act
        Login(username, password);

        // Assert
        WaitForElement(By.Id("dashboard"));
        Assert.That(Driver.Url, Does.Contain("/Home/Dashboard"));
    }

    [Test]
    public void Login_WithInvalidCredentials_ShouldShowError()
    {
        // Arrange
        var username = "invaliduser";
        var password = "invalidpass";

        // Act
        Login(username, password);

        // Assert
        WaitForElement(By.ClassName("alert-danger"));
        var errorMessage = Driver.FindElement(By.ClassName("alert-danger")).Text;
        Assert.That(errorMessage, Does.Contain("Invalid username or password"));
    }

    [Test]
    public void Login_WithEmptyFields_ShouldShowValidationErrors()
    {
        // Act
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("validation-summary-errors"));
        var validationErrors = Driver.FindElement(By.ClassName("validation-summary-errors")).Text;
        Assert.That(validationErrors, Does.Contain("Username is required"));
        Assert.That(validationErrors, Does.Contain("Password is required"));
    }
} 