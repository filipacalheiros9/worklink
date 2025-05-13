using Microsoft.AspNetCore.Mvc;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Entities;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult GetTarefas()
        {
            var tarefas = _context.Tarefas.Where(t => t.IdProjeto == 0).ToList();
            return Ok(tarefas);
        }

        [HttpGet("{id}")]
        public IActionResult GetTarefa(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null) return NotFound();
            return Ok(tarefa);
        }

        [HttpPost]
        public IActionResult CreateTarefa([FromBody] TarefasIndCreate dto)
        {
            var tarefa = new Tarefa
            {
                NomeTarefa = dto.NomeTarefa,
                DtInicio = dto.DtInicio,
                HrInicio = dto.HrInicio,
                DtFim = dto.DtFim,
                HrFim = dto.HrFim,
                IdProjeto = 0
            };

            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetTarefa), new { id = tarefa.IdTarefa }, tarefa);
        }

        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaCreate dto)
        {
            try
            {
                var tarefa = await _context.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == id);
                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada." });

                tarefa.NomeTarefa = dto.NomeTarefa;
                tarefa.DtFim = dto.DtFim;
                tarefa.HrFim = dto.HrFim;

                await _context.SaveChangesAsync();
                return Ok(new { message = "Tarefa atualizada com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ERRO AO ATUALIZAR TAREFA:");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    message = "Erro interno ao atualizar a tarefa.",
                    detalhe = ex.Message
                });
            }
        }

        [HttpPost("CopiarTarefa/{id}")]
        public async Task<IActionResult> CopiarTarefa(int id, [FromQuery] decimal novoProjetoId)
        {
            try
            {
                var tarefaOriginal = await _context.Tarefas.FirstOrDefaultAsync(t => t.IdTarefa == id);
                if (tarefaOriginal == null)
                    return NotFound(new { message = "Tarefa original não encontrada." });

                var novaTarefa = new Tarefa
                {
                    NomeTarefa = tarefaOriginal.NomeTarefa,
                    DtInicio = tarefaOriginal.DtInicio,
                    HrInicio = tarefaOriginal.HrInicio,
                    DtFim = tarefaOriginal.DtFim,
                    HrFim = tarefaOriginal.HrFim,
                    IdProjeto = novoProjetoId
                };

                _context.Tarefas.Add(novaTarefa);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tarefa copiada com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ERRO AO COPIAR TAREFA:");
                Console.WriteLine($"Mensagem: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                return StatusCode(500, new
                {
                    message = "Erro interno ao copiar a tarefa.",
                    detalhe = ex.Message
                });
            }
        }


        [HttpDelete("{id}")]
        public IActionResult DeleteTarefa(int id)
        {
            var tarefa = _context.Tarefas.FirstOrDefault(t => t.IdTarefa == id);
            if (tarefa == null) return NotFound();

            _context.Tarefas.Remove(tarefa);
            _context.SaveChanges();
            return NoContent();
        }
    }
}
