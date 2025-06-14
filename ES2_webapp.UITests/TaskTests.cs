using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace ES2_webapp.UITests;

[TestFixture]
public class TaskTests : TestBase
{
    private string _projectName;

    [SetUp]
    public void Setup()
    {
        base.Setup();
        Login("testuser", "testpass");
        WaitForElement(By.Id("dashboard"));

        // Create a test project for task tests
        _projectName = "Test Project " + DateTime.Now.Ticks;
        CreateTestProject();
    }

    private void CreateTestProject()
    {
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText("Create New"));
        Driver.FindElement(By.LinkText("Create New")).Click();
        
        WaitForElement(By.Id("Name"));
        Driver.FindElement(By.Id("Name")).SendKeys(_projectName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));
    }

    [Test]
    public void CreateTask_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var taskName = "Test Task " + DateTime.Now.Ticks;
        var taskDescription = "Test Task Description";

        // Act
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText("Add Task"));
        Driver.FindElement(By.LinkText("Add Task")).Click();

        WaitForElement(By.Id("Title"));
        Driver.FindElement(By.Id("Title")).SendKeys(taskName);
        Driver.FindElement(By.Id("Description")).SendKeys(taskDescription);
        
        // Select priority from dropdown
        var prioritySelect = new SelectElement(Driver.FindElement(By.Id("Priority")));
        prioritySelect.SelectByText("High");

        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Task created successfully"));
        
        // Verify task appears in list
        Assert.That(Driver.PageSource, Does.Contain(taskName));
    }

    [Test]
    public void EditTask_WithValidData_ShouldUpdateSuccessfully()
    {
        // Arrange
        var taskName = "Test Task " + DateTime.Now.Ticks;
        var updatedName = "Updated Task " + DateTime.Now.Ticks;

        // Create a task first
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText("Add Task"));
        Driver.FindElement(By.LinkText("Add Task")).Click();

        WaitForElement(By.Id("Title"));
        Driver.FindElement(By.Id("Title")).SendKeys(taskName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));

        // Act - Edit the task
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText(taskName));
        Driver.FindElement(By.LinkText(taskName)).Click();
        WaitForElement(By.LinkText("Edit"));
        Driver.FindElement(By.LinkText("Edit")).Click();

        WaitForElement(By.Id("Title"));
        Driver.FindElement(By.Id("Title")).Clear();
        Driver.FindElement(By.Id("Title")).SendKeys(updatedName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Task updated successfully"));
        
        // Verify updated name appears in list
        Assert.That(Driver.PageSource, Does.Contain(updatedName));
    }

    [Test]
    public void DeleteTask_ShouldRemoveFromList()
    {
        // Arrange
        var taskName = "Test Task " + DateTime.Now.Ticks;

        // Create a task first
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText("Add Task"));
        Driver.FindElement(By.LinkText("Add Task")).Click();

        WaitForElement(By.Id("Title"));
        Driver.FindElement(By.Id("Title")).SendKeys(taskName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));

        // Act - Delete the task
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText(taskName));
        Driver.FindElement(By.LinkText(taskName)).Click();
        WaitForElement(By.LinkText("Delete"));
        Driver.FindElement(By.LinkText("Delete")).Click();
        
        WaitForElement(By.CssSelector("button[type='submit']"));
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Task deleted successfully"));
        
        // Verify task no longer appears in list
        Assert.That(Driver.PageSource, Does.Not.Contain(taskName));
    }

    [Test]
    public void ChangeTaskStatus_ShouldUpdateSuccessfully()
    {
        // Arrange
        var taskName = "Test Task " + DateTime.Now.Ticks;

        // Create a task first
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText("Add Task"));
        Driver.FindElement(By.LinkText("Add Task")).Click();

        WaitForElement(By.Id("Title"));
        Driver.FindElement(By.Id("Title")).SendKeys(taskName);
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();
        WaitForElement(By.ClassName("alert-success"));

        // Act - Change task status
        Driver.FindElement(By.LinkText("Projects")).Click();
        WaitForElement(By.LinkText(_projectName));
        Driver.FindElement(By.LinkText(_projectName)).Click();
        WaitForElement(By.LinkText(taskName));
        Driver.FindElement(By.LinkText(taskName)).Click();
        
        var statusSelect = new SelectElement(Driver.FindElement(By.Id("Status")));
        statusSelect.SelectByText("In Progress");
        Driver.FindElement(By.CssSelector("button[type='submit']")).Click();

        // Assert
        WaitForElement(By.ClassName("alert-success"));
        var successMessage = Driver.FindElement(By.ClassName("alert-success")).Text;
        Assert.That(successMessage, Does.Contain("Task updated successfully"));
        
        // Verify status is updated
        Assert.That(Driver.PageSource, Does.Contain("In Progress"));
    }
} 