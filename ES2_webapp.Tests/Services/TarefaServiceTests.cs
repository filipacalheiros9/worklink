using Moq;
using NUnit.Framework;
using WebApplication2.Data.Repositories;
using WebApplication2.Entities;
using WebApplication2.Services;

namespace ES2_webapp.Tests.Services;

[TestFixture]
public class TarefaServiceTests
{
    private Mock<ITarefaRepository> _mockTarefaRepository;
    private TarefaService _tarefaService;

    [SetUp]
    public void Setup()
    {
        _mockTarefaRepository = new Mock<ITarefaRepository>();
        _tarefaService = new TarefaService(_mockTarefaRepository.Object);
    }

    [Test]
    public async Task GetTarefaByIdAsync_WhenTarefaExists_ReturnsTarefa()
    {
        // Arrange
        var expectedTarefa = new Tarefa { IdTarefa = 1, NomeTarefa = "Test Task" };
        _mockTarefaRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(expectedTarefa);

        // Act
        var result = await _tarefaService.GetTarefaByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IdTarefa, Is.EqualTo(expectedTarefa.IdTarefa));
        Assert.That(result.NomeTarefa, Is.EqualTo(expectedTarefa.NomeTarefa));
    }

    [Test]
    public async Task GetAllTarefasAsync_ReturnsAllTarefas()
    {
        // Arrange
        var expectedTarefas = new List<Tarefa>
        {
            new() { IdTarefa = 1, NomeTarefa = "Task 1" },
            new() { IdTarefa = 2, NomeTarefa = "Task 2" }
        };
        _mockTarefaRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedTarefas);

        // Act
        var result = await _tarefaService.GetAllTarefasAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public async Task AddTarefaAsync_CallsRepositoryAdd()
    {
        // Arrange
        var tarefa = new Tarefa { IdTarefa = 1, NomeTarefa = "New Task" };
        _mockTarefaRepository.Setup(repo => repo.AddAsync(tarefa))
            .Returns(Task.CompletedTask);

        // Act
        await _tarefaService.AddTarefaAsync(tarefa);

        // Assert
        _mockTarefaRepository.Verify(repo => repo.AddAsync(tarefa), Times.Once);
    }

    [Test]
    public async Task UpdateTarefaAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var tarefa = new Tarefa { IdTarefa = 1, NomeTarefa = "Updated Task" };
        _mockTarefaRepository.Setup(repo => repo.UpdateAsync(tarefa))
            .Returns(Task.CompletedTask);

        // Act
        await _tarefaService.UpdateTarefaAsync(tarefa);

        // Assert
        _mockTarefaRepository.Verify(repo => repo.UpdateAsync(tarefa), Times.Once);
    }

    [Test]
    public async Task DeleteTarefaAsync_CallsRepositoryDelete()
    {
        // Arrange
        var tarefaId = 1;
        _mockTarefaRepository.Setup(repo => repo.DeleteAsync(tarefaId))
            .Returns(Task.CompletedTask);

        // Act
        await _tarefaService.DeleteTarefaAsync(tarefaId);

        // Assert
        _mockTarefaRepository.Verify(repo => repo.DeleteAsync(tarefaId), Times.Once);
    }
} 