using Moq;
using NUnit.Framework;
using WebApplication2.Data.Repositories;
using WebApplication2.Entities;
using WebApplication2.Services;

namespace ES2_webapp.Tests.Services;

[TestFixture]
public class ProjetoServiceTests
{
    private Mock<IProjetoRepositorio> _mockProjetoRepository;
    private ProjetoService _projetoService;

    [SetUp]
    public void Setup()
    {
        _mockProjetoRepository = new Mock<IProjetoRepositorio>();
        _projetoService = new ProjetoService(_mockProjetoRepository.Object);
    }

    [Test]
    public async Task GetProjetoByIdAsync_WhenProjetoExists_ReturnsProjeto()
    {
        // Arrange
        var expectedProjeto = new Projeto { IdProjeto = 1, NomeProjeto = "Test Project" };
        _mockProjetoRepository.Setup(repo => repo.GetByIdAsync(1))
            .ReturnsAsync(expectedProjeto);

        // Act
        var result = await _projetoService.GetProjetoByIdAsync(1);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.IdProjeto, Is.EqualTo(expectedProjeto.IdProjeto));
        Assert.That(result.NomeProjeto, Is.EqualTo(expectedProjeto.NomeProjeto));
    }

    [Test]
    public async Task GetAllProjetosAsync_ReturnsAllProjetos()
    {
        // Arrange
        var expectedProjetos = new List<Projeto>
        {
            new() { IdProjeto = 1, NomeProjeto = "Project 1" },
            new() { IdProjeto = 2, NomeProjeto = "Project 2" }
        };
        _mockProjetoRepository.Setup(repo => repo.GetAllAsync())
            .ReturnsAsync(expectedProjetos);

        // Act
        var result = await _projetoService.GetAllProjetosAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(2));
    }

    [Test]
    public void ObterProjetosPessoais_ReturnsPersonalProjects()
    {
        // Arrange
        var userId = 1m;
        var expectedProjetos = new List<Projeto>
        {
            new() { IdProjeto = 1, NomeProjeto = "Personal Project 1" },
            new() { IdProjeto = 2, NomeProjeto = "Personal Project 2" }
        };
        _mockProjetoRepository.Setup(repo => repo.ObterProjetosPessoais(userId))
            .Returns(expectedProjetos);

        // Act
        var result = _projetoService.ObterProjetosPessoais(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(p => p.NomeProjeto.StartsWith("Personal")), Is.True);
    }

    [Test]
    public void ObterProjetosEquipa_ReturnsTeamProjects()
    {
        // Arrange
        var userId = 1m;
        var expectedProjetos = new List<Projeto>
        {
            new() { IdProjeto = 1, NomeProjeto = "Team Project 1" },
            new() { IdProjeto = 2, NomeProjeto = "Team Project 2" }
        };
        _mockProjetoRepository.Setup(repo => repo.ObterProjetosEquipa(userId))
            .Returns(expectedProjetos);

        // Act
        var result = _projetoService.ObterProjetosEquipa(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(p => p.NomeProjeto.StartsWith("Team")), Is.True);
    }

    [Test]
    public void ObterProjetosVisiveis_CombinesPersonalAndTeamProjects()
    {
        // Arrange
        var userId = 1m;
        var personalProjects = new List<Projeto>
        {
            new() { IdProjeto = 1, NomeProjeto = "Personal Project" }
        };
        var teamProjects = new List<Projeto>
        {
            new() { IdProjeto = 2, NomeProjeto = "Team Project" }
        };
        _mockProjetoRepository.Setup(repo => repo.ObterProjetosPessoais(userId))
            .Returns(personalProjects);
        _mockProjetoRepository.Setup(repo => repo.ObterProjetosEquipa(userId))
            .Returns(teamProjects);

        // Act
        var result = _projetoService.ObterProjetosVisiveis(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Any(p => p.NomeProjeto == "Personal Project"), Is.True);
        Assert.That(result.Any(p => p.NomeProjeto == "Team Project"), Is.True);
    }

    [Test]
    public async Task GetAllWithCriadorAsync_ReturnsProjectsWithCreator()
    {
        // Arrange
        var expectedProjetos = new List<Projeto>
        {
            new() { IdProjeto = 1, NomeProjeto = "Project 1", Criador = new Utilizador { IdUtilizador = 1, Nome = "Creator 1" } },
            new() { IdProjeto = 2, NomeProjeto = "Project 2", Criador = new Utilizador { IdUtilizador = 2, Nome = "Creator 2" } }
        };
        _mockProjetoRepository.Setup(repo => repo.GetAllWithCriadorAsync())
            .ReturnsAsync(expectedProjetos);

        // Act
        var result = await _projetoService.GetAllWithCriadorAsync();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.All(p => p.Criador != null), Is.True);
    }
} 