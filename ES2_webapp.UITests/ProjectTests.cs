using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ES2_webapp.UITests;

[TestFixture]
public class ProjectTests : TestBase
{
    [SetUp]
    public void Setup()
    {
        base.Setup();
        Login("testuser", "testpass");
        WaitForElement(By.Id("dashboard"));
    }

    [Test]
    public void CreateProject_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var projectName = "Test Project " + DateTime.Now.Ticks;
        var projectDescription = "Test Description";

        // Act
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText("Create New"));
        Driver.FindElement(By.LinkText("Create New")).Click();
        
        WaitForElement(By.Id("Name"));
        Driver.FindElement(By.Id("Name")).SendKeys(projectName);
        Driver.FindElement(By.Id("Description")).SendKeys(projectDescription);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Project created successfully"));
        
        // Verify project appears in list
        Assert.That(Driver.PageSource, Does.Contain(projectName));
    }

    [Test]
    public void EditProject_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var projectName = "Test Project " + DateTime.Now.Ticks;
        var updatedName = "Updated Project " + DateTime.Now.Ticks;

        // Create a project first
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText("Create New"));
        Driver.FindElement(By.LinkText("Create New")).Click();
        
        WaitForElement(By.Id("Name"));
        Driver.FindElement(By.Id("Name")).SendKeys(projectName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));

        // Act - Edit the project
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(projectName));
        Driver.FindElement(By.LinkText(projectName)).Click();
        WaitForElement(By.LinkText("Edit"));
        Driver.FindElement(By.LinkText("Edit")).Click();

        WaitForElement(By.Id("Name"));
        Driver.FindElement(By.Id("Name")).Clear();
        Driver.FindElement(By.Id("Name")).SendKeys(updatedName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Project updated successfully"));
        
        // Verify updated name appears in list
        Assert.That(Driver.PageSource, Does.Contain(updatedName));
    }

    [Test]
    public void DeleteProject_ShouldRemoveFromList()
    {
        // Arrange
        var projectName = "Test Project " + DateTime.Now.Ticks;

        // Create a project first
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText("Create New"));
        Driver.FindElement(By.LinkText("Create New")).Click();
        
        WaitForElement(By.Id("Name"));
        Driver.FindElement(By.Id("Name")).SendKeys(projectName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));

        // Act - Delete the project
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(projectName));
        Driver.FindElement(By.LinkText(projectName)).Click();
        WaitForElement(By.LinkText("Delete"));
        Driver.FindElement(By.LinkText("Delete")).Click();
        
        WaitForElement(By.CssSelector("button[type='submit']"));
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Project deleted successfully"));
        
        // Verify project no longer appears in list
        Assert.That(Driver.PageSource, Does.Not.Contain(projectName));
    }
} 