using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;
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
        public async Task<IActionResult> Index(decimal projetoId)
        {
            var projeto = await _context.Projetos.FindAsync(projetoId);
            if (projeto == null) return NotFound();

            ViewBag.ProjetoId = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;

            return View("~/Views/Home/Tarefas.cshtml"); 
        }
        
        
        [HttpPost("CriarTarefa")]
        public async Task<IActionResult> CriarTarefa([FromBody] TarefaCreate dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos." });

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
                return Unauthorized(new { message = "Utilizador não autenticado." });

            if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return BadRequest(new { message = "ID do utilizador inválido." });

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == dto.IdProjeto && p.IdUtilizador == idUtilizador);
            if (projeto == null)
                return NotFound(new { message = "Projeto não encontrado ou não pertence ao utilizador." });

            var tarefa = new Tarefa
            {
                NomeTarefa = dto.NomeTarefa,
                DtInicio = dto.DtInicio,
                HrInicio = dto.HrInicio,
                IdProjeto = dto.IdProjeto
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tarefa criada com sucesso.",
                idTarefa = tarefa.IdTarefa, 
                nomeTarefa = tarefa.NomeTarefa,
                dtInicio = tarefa.DtInicio
            });
        }



        [HttpGet("MinhasTarefas/{idProjeto}")]
        public async Task<IActionResult> GetTarefasPorProjeto(decimal idProjeto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
                return Unauthorized(new { message = "Utilizador não autenticado." });

            if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return BadRequest(new { message = "ID do utilizador inválido." });

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == idProjeto && p.IdUtilizador == idUtilizador);

            if (projeto == null)
                return NotFound(new { message = "Projeto não encontrado ou não pertence ao utilizador." });

            var tarefas = await _context.Tarefas
                .Where(t => t.IdProjeto == idProjeto)
                .ToListAsync();

            return Ok(tarefas);
        }


        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaCreate dto)
            {
                try
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                    
                    var tarefa = await _context.Tarefas
                        .Include(t => t.Projeto)
                        .FirstOrDefaultAsync(t => t.IdTarefa == id);
                   
                    tarefa.NomeTarefa = dto.NomeTarefa;
                    tarefa.DtFim = dto.DtFim;
                    tarefa.HrFim = dto.HrFim;
                    
                    
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "Tarefa atualizada com sucesso." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine("❌ ERRO AO ATUALIZAR TAREFA:");
                    Console.WriteLine($"Mensagem: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    return StatusCode(500, new
                    {
                        message = "Erro interno ao atualizar a tarefa.",
                        detalhe = ex.Message
                    });
                }
            }


        [HttpDelete("EliminarTarefa/{id}")]
        public async Task<IActionResult> EliminarTarefa(int id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return BadRequest(new { message = "ID do utilizador inválido." });

                var tarefa = await _context.Tarefas
                    .Include(t => t.Projeto)
                    .FirstOrDefaultAsync(t => t.IdTarefa == id && t.Projeto.IdUtilizador == idUtilizador);

                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada ou não pertence ao utilizador." });

                _context.Tarefas.Remove(tarefa);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarefa eliminada com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO ELIMINAR TAREFA:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Erro interno", detalhe = ex.Message });
            }
        }
    }
}

