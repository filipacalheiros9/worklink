using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Entities;
using System.Linq;
using System.Threading.Tasks;
using ES2_webapp.Data;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/TarefaIND")]
    public class TarefaINDController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TarefaINDController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTarefas()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var tarefas = await _context.Tarefas
                .Where(t => t.IdUtilizador == userId)
                .ToListAsync();

            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTarefa([FromBody] TarefasIndCreate dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var tarefa = new Tarefa
            {
                NomeTarefa = dto.NomeTarefa,
                DtInicio = dto.DtInicio,
                HrInicio = dto.HrInicio,
                DtFim = dto.DtFim,
                HrFim = dto.HrFim,
                PrecoHora = dto.PrecoHora,
                IdUtilizador = userId
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.IdTarefa }, tarefa);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            return tarefa == null ? NotFound() : Ok(tarefa);
        }

        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaCreate dto)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null) return NotFound();

            tarefa.NomeTarefa = dto.NomeTarefa;
            tarefa.DtFim = dto.DtFim;
            tarefa.HrFim = dto.HrFim;
            tarefa.PrecoHora = dto.PrecoHora;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Tarefa atualizada com sucesso." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTarefa(int id)
        {
            var tarefa = await _context.Tarefas.FindAsync(id);
            if (tarefa == null) return NotFound();

            _context.Tarefas.Remove(tarefa);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("ProjetosParaExportar")]
        public async Task<IActionResult> ProjetosParaExportar()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var projetos = await _context.Projetos
                .Where(p => p.IdUtilizador == userId)
                .Select(p => new { p.IdProjeto, p.NomeProjeto })
                .ToListAsync();

            return Ok(projetos);
        }

        [HttpPost("CopiarTarefa/{idTarefa}")]
        public async Task<IActionResult> CopiarTarefa(int idTarefa, [FromQuery] decimal idProjeto)
        {
            var tarefaOriginal = await _context.Tarefas.FindAsync(idTarefa);
            if (tarefaOriginal == null)
                return NotFound(new { message = "Tarefa original não encontrada." });

            var projetoExiste = await _context.Projetos.FindAsync(idProjeto);
            if (projetoExiste == null)
                return NotFound(new { message = "Projeto de destino não encontrado." });

            var novaTarefa = new Tarefa
            {
                NomeTarefa = tarefaOriginal.NomeTarefa,
                DtInicio = tarefaOriginal.DtInicio,
                HrInicio = tarefaOriginal.HrInicio,
                DtFim = tarefaOriginal.DtFim,
                HrFim = tarefaOriginal.HrFim,
                PrecoHora = tarefaOriginal.PrecoHora,
                IdUtilizador = tarefaOriginal.IdUtilizador
            };

            _context.Tarefas.Add(novaTarefa);
            await _context.SaveChangesAsync();

            var relacao = new ProjetoTarefa
            {
                IdProjeto = idProjeto,
                TarefaId = novaTarefa.IdTarefa
            };

            _context.ProjetoTarefa.Add(relacao);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Tarefa copiada com sucesso para o projeto." });
        }
    }
}
