using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TarefasApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TarefasApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("projeto/{idProjeto}")]
        public async Task<IActionResult> GetTarefasPorProjeto(decimal idProjeto)
        {
            var tarefas = await _context.Tarefas
                .Where(t => t.IdProjeto == idProjeto)
                .Select(t => new TarefaDTO
                {
                    NomeTarefa = t.NomeTarefa,
                    DtInicio = t.DtInicio,
                    HrInicio = t.HrInicio,
                    DtFim = t.DtFim,
                    HrFim = t.HrFim,
                    IdProjeto = t.IdProjeto
                })
                .ToListAsync();

            return Ok(tarefas);
        }

        [HttpPost]
        public async Task<IActionResult> CriarTarefa([FromBody] TarefaDTO dto)
        {
            var tarefa = new Tarefa
            {
                NomeTarefa = dto.NomeTarefa,
                DtInicio = dto.DtInicio,
        
                // Converte string para TimeSpan
                HrInicio =dto.HrInicio,
        
                DtFim = dto.DtFim,
        
                // Converte string para TimeSpan
                HrFim = dto.HrFim,
        
                IdProjeto = dto.IdProjeto
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            return Ok(dto); // devolve a tarefa criada
        }

        public class TarefaDTO
        {
            public string NomeTarefa { get; set; } = string.Empty;
            public DateOnly? DtInicio { get; set; }
            public string? HrInicio { get; set; }
            public DateOnly? DtFim { get; set; }
            public string? HrFim { get; set; }
            public decimal IdProjeto { get; set; }
        }
    }
}
