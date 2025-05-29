using ES2_webapp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using WebApplication2.DTO;

namespace WebApplication2.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProjetoEquipas: Controller
{
    private readonly ApplicationDbContext _context;
    
    public ProjetoEquipas(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public IActionResult Index(int idEquipa)
    {
        ViewBag.IdEquipa = idEquipa; 
        return View();
    } 
    

    [HttpPost("CriarProjetoEquipa")]
    public async Task<IActionResult> CriarProjetoEquipa([FromBody] CriarProjetoDto dto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (!decimal.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var membro = await _context.EquipaUtilizadores
            .AnyAsync(eu => eu.EquipaId == dto.EquipaId && eu.UtilizadorId == userId);

        if (!membro)
            return Forbid("Não pertences a esta equipa.");

        var projeto = new Projeto
        {
            NomeProjeto = dto.Nome,
            NomeCliente = dto.Nome,
            IdUtilizador = userId,
            EquipaId = dto.EquipaId
        };

        _context.Projetos.Add(projeto);
        await _context.SaveChangesAsync();

        return Ok(new { projeto.IdProjeto, projeto.NomeProjeto });
    }


    [HttpGet("PorEquipa/{equipaId}")]
    public async Task<IActionResult> ListarProjetosPorEquipa(int equipaId)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (!decimal.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var membro = await _context.EquipaUtilizadores
            .AnyAsync(eu => eu.EquipaId == equipaId && eu.UtilizadorId == userId);

        if (!membro)
            return Forbid("Não pertences a esta equipa.");

        var projetos = await _context.Projetos
            .Where(p => p.EquipaId == equipaId)
            .Select(p => new
            {
                p.IdProjeto,
                p.NomeProjeto,
                p.NomeCliente,
                Criador = p.Criador.Username
            })
            .ToListAsync();

        return Ok(projetos);
    }


    [HttpPut("AtualizarProjetoEquipa/{id}")]
    public async Task<IActionResult> AtualizarProjetoEquipa(int id, [FromBody] ProjetoCreate dto)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (!decimal.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.IdProjeto == id);

        if (projeto == null || projeto.IdUtilizador != userId)
            return Forbid("Não és o criador deste projeto.");

        projeto.NomeProjeto = dto.NomeProjeto;
        projeto.NomeCliente = dto.NomeCliente;

        await _context.SaveChangesAsync();
        return Ok(new { message = "Projeto atualizado com sucesso." });
    }


    [HttpDelete("EliminarProjetoEquipa/{id}")]
    public async Task<IActionResult> EliminarProjetoEquipa(int id)
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
        if (!decimal.TryParse(userIdClaim, out var userId))
            return Unauthorized();

        var projeto = await _context.Projetos
            .Include(p => p.ProjetosTarefas)
            .ThenInclude(pt => pt.Tarefa)
            .FirstOrDefaultAsync(p => p.IdProjeto == id);

        if (projeto == null || projeto.IdUtilizador != userId)
            return Forbid("Não és o criador deste projeto.");

        foreach (var pt in projeto.ProjetosTarefas)
        {
            _context.Tarefas.Remove(pt.Tarefa);
        }

        _context.ProjetoTarefa.RemoveRange(projeto.ProjetosTarefas);
        _context.Projetos.Remove(projeto);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Projeto eliminado com sucesso." });
    }
}

