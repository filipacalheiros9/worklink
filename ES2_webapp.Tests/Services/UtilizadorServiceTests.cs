using Moq;
using NUnit.Framework;
using WebApplication2.Data.Repositories;
using WebApplication2.DTO;
using WebApplication2.Entities;
using WebApplication2.Services;

namespace ES2_webapp.Tests.Services;

[TestFixture]
public class UtilizadorServiceTests
{
    private Mock<IUtilizadorRepository> _mockUtilizadorRepository;
    private UtilizadorService _utilizadorService;

    [SetUp]
    public void Setup()
    {
        _mockUtilizadorRepository = new Mock<IUtilizadorRepository>();
        _utilizadorService = new UtilizadorService(_mockUtilizadorRepository.Object);
    }

    [Test]
    public async Task GetUtilizadorByIdAsync_WhenUtilizadorExists_ReturnsUtilizador()
    {
        // Arrange
        var expectedUtilizador = new Utilizador { IdUtilizador = 1, Nome = "Test User", Username = "testuser" };
        _mockUtilizadorRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(expectedUtilizador);

        // Act
        var result = await _utilizadorService.GetUtilizadorByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IdUtilizador, Is.EqualTo(expectedUtilizador.IdUtilizador));
        Assert.That(result.Nome, Is.EqualTo(expectedUtilizador.Nome));
        Assert.That(result.Username, Is.EqualTo(expectedUtilizador.Username));
    }

    [Test]
    public async Task ValidateLoginAsync_WithValidCredentials_ReturnsUtilizador()
    {
        // Arrange
        var username = "testuser";
        var password = "password123";
        var expectedUtilizador = new Utilizador { IdUtilizador = 1, Username = username, Password = password };
        _mockUtilizadorRepository.Setup(repo => repo.GetByCredentialsAsync(username, password))
            .ReturnsAsync(expectedUtilizador);

        // Act
        var result = await _utilizadorService.ValidateLoginAsync(username, password);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo(username));
    }

    [Test]
    public async Task RegisterUtilizadorAsync_CallsRepositoryAdd()
    {
        // Arrange
        var utilizador = new Utilizador { IdUtilizador = 1, Nome = "New User", Username = "newuser" };
        _mockUtilizadorRepository.Setup(repo => repo.AddAsync(utilizador))
            .Returns(Task.CompletedTask);

        // Act
        await _utilizadorService.RegisterUtilizadorAsync(utilizador);

        // Assert
        _mockUtilizadorRepository.Verify(repo => repo.AddAsync(utilizador), Times.Once);
    }

    [Test]
    public async Task AtualizarPerfil_WhenUtilizadorExists_UpdatesProfile()
    {
        // Arrange
        var userId = 1m;
        var existingUtilizador = new Utilizador { IdUtilizador = userId, Nome = "Old Name", Username = "olduser" };
        var updateModel = new UtilizadorUpdate { Nome = "New Name", Username = "newuser", Password = "newpass" };

        _mockUtilizadorRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync(existingUtilizador);
        _mockUtilizadorRepository.Setup(repo => repo.AtualizarUtilizador(It.IsAny<Utilizador>()))
            .Returns(Task.CompletedTask);

        // Act
        await _utilizadorService.AtualizarPerfil(userId, updateModel);

        // Assert
        _mockUtilizadorRepository.Verify(repo => repo.AtualizarUtilizador(It.Is<Utilizador>(u =>
            u.IdUtilizador == userId &&
            u.Nome == updateModel.Nome &&
            u.Username == updateModel.Username &&
            u.Password == updateModel.Password)), Times.Once);
    }

    [Test]
    public async Task AtualizarPerfil_WhenUtilizadorDoesNotExist_ThrowsException()
    {
        // Arrange
        var userId = 1m;
        var updateModel = new UtilizadorUpdate { Nome = "New Name", Username = "newuser", Password = "newpass" };

        _mockUtilizadorRepository.Setup(repo => repo.GetByIdAsync(userId))
            .ReturnsAsync((Utilizador)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(() => _utilizadorService.AtualizarPerfil(userId, updateModel));
        Assert.That(ex.Message, Is.EqualTo("Utilizador não encontrado."));
    }

    [Test]
    public async Task CountUtilizadoresAsync_ReturnsCorrectCount()
    {
        // Arrange
        var expectedCount = 5;
        _mockUtilizadorRepository.Setup(repo => repo.CountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _utilizadorService.CountUtilizadoresAsync();

        // Assert
        Assert.That(result, Is.EqualTo(expectedCount));
    }

    [Test]
    public async Task GetAllUtilizadoresAsync_ReturnsAllUtilizadores()
    {
        // Arrange
        var expectedUtilizadores = new List<Utilizador>
        {
            new() { IdUtilizador = 1, Nome = "User 1" },
            new() { IdUtilizador = 2, Nome = "User 2" }
        };
        _mockUtilizadorRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedUtilizadores);

        // Act
        var result = await _utilizadorService.GetAllUtilizadoresAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
    }

    [Test]
    public async Task DeleteUtilizadorAsync_CallsRepositoryDelete()
    {
        // Arrange
        var userId = 1m;
        _mockUtilizadorRepository.Setup(repo => repo.DeleteAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        await _utilizadorService.DeleteUtilizadorAsync(userId);

        // Assert
        _mockUtilizadorRepository.Verify(repo => repo.DeleteAsync(userId), Times.Once);
    }
} 