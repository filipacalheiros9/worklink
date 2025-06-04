using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.Data;
using WebApplication2.DTO;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("Tarefas")]
    public class TarefasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(int projetoId) // ✅ mudou para int
        {
            var projeto = await _context.Projetos.FindAsync(projetoId);
            if (projeto == null) return NotFound();

            ViewBag.ProjetoId = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;
            return View("~/Views/Home/Tarefas.cshtml");
        }
        [HttpPut("MoverTarefa/{idTarefa}")]
        public async Task<IActionResult> MoverTarefa(int idTarefa, [FromBody] int novaFase)
        {
            var projetoTarefa = await _context.ProjetoTarefa
                .FirstOrDefaultAsync(pt => pt.TarefaId == idTarefa);

            if (projetoTarefa == null)
                return NotFound();

            projetoTarefa.Fase = novaFase;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fase atualizada com sucesso." });
        }


        [HttpGet("MinhasTarefas/{idProjeto}")]
        public async Task<IActionResult> GetTarefasPorProjeto(int idProjeto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == idProjeto && p.IdUtilizador == idUtilizador);

            if (projeto == null)
                return NotFound();

            var tarefas = await _context.ProjetoTarefa
                .Where(pt => pt.IdProjeto == idProjeto)
                .Select(pt => new {
                    pt.Tarefa.IdTarefa,
                    pt.Tarefa.NomeTarefa,
                    pt.Tarefa.PrecoHora,
                    pt.Tarefa.DtFim,
                    pt.Tarefa.HrFim,
                    pt.Fase // <- necessário para o kanban
                })
                .ToListAsync();

            return Ok(tarefas);
        }


        [HttpPost("CriarTarefa")]
        public async Task<IActionResult> CriarTarefa([FromBody] TarefaCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos." });

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized(new { message = "Utilizador não autenticado." });

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == dto.IdProjeto && p.IdUtilizador == idUtilizador);

            if (projeto == null)
                return NotFound(new { message = "Projeto não encontrado ou não pertence ao utilizador." });

            var tarefa = new Tarefa
            {
                NomeTarefa = dto.NomeTarefa,
                DtInicio = dto.DtInicio,
                HrInicio = dto.HrInicio,
                DtFim = dto.DtFim,
                HrFim = dto.HrFim,
                PrecoHora = dto.PrecoHora,
                IdUtilizador = null // ✅ porque está ligada à equipa/projeto
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            var projetoTarefa = new ProjetoTarefa
            {
                IdProjeto = dto.IdProjeto,
                TarefaId = tarefa.IdTarefa
            };

            _context.ProjetoTarefa.Add(projetoTarefa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tarefa criada com sucesso.",
                idTarefa = tarefa.IdTarefa,
                nomeTarefa = tarefa.NomeTarefa,
                PrecoHora = tarefa.PrecoHora,
                dtInicio = tarefa.DtInicio
            });
        }

        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaEditDto dto)
        {
            try
            {
                var tarefa = await _context.Tarefas.FindAsync(id);
                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada." });

                tarefa.NomeTarefa = dto.NomeTarefa;
                tarefa.DtFim = dto.DtFim;
                tarefa.HrFim = dto.HrFim ?? tarefa.HrFim;
                tarefa.PrecoHora = dto.PrecoHora;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Tarefa atualizada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno ao atualizar a tarefa.",
                    detalhe = ex.Message
                });
            }
        }

        [HttpDelete("EliminarProjeto/{id}")] // ❗este método devia estar noutro controller, mas vamos manter
        public async Task<IActionResult> EliminarProjeto(int id) // ✅ mudou para int
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return Unauthorized();

                var projeto = await _context.Projetos
                    .Include(p => p.ProjetosTarefas)
                    .ThenInclude(pt => pt.Tarefa)
                    .FirstOrDefaultAsync(p => p.IdProjeto == id && p.IdUtilizador == idUtilizador);

                if (projeto == null)
                    return NotFound(new { message = "Projeto não encontrado ou não pertence ao utilizador." });

                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Projeto e tarefas associadas eliminados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao eliminar projeto", detalhe = ex.Message });
            }
        }
    }
}
