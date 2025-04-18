using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;
using WebApplication2.Models;
using WebApplication2.Data; 

    namespace WebApplication2.Controllers
    {
        [Route("api/[controller]")]
        [ApiController]
        public class ProjetoAPIController : ControllerBase
        {
            private readonly ApplicationDbContext _context; 

            public ProjetoAPIController(ApplicationDbContext context)
            {
                _context = context;
            }


            [HttpPost]
            public async Task<ActionResult<Projeto>> CriarProjeto([FromBody] ProjetoCreateDTO projetoDTO)
            {
                var projeto = new Projeto
                {
                    NomeProjeto = projetoDTO.NomeProjeto,
                    NomeCliente = projetoDTO.NomeCliente,
                    PrecoHora = projetoDTO.PrecoHora,
                    IdUtilizador = projetoDTO.IdUtilizador
                };

                _context.Projetos.Add(projeto);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(ObterProjetoPorId), new { id = projeto.IdProjeto }, projeto);
            }

         
            [HttpGet]
            public async Task<ActionResult<IEnumerable<Projeto>>> ListarProjetos()
            {
                var projetos = await _context.Projetos.ToListAsync();
                return Ok(projetos);
            }

           
            [HttpGet("{id}")]
            public async Task<ActionResult<Projeto>> ObterProjetoPorId(decimal id)
            {
                var projeto = await _context.Projetos.FindAsync(id);

                if (projeto == null)
                    return NotFound();

                return Ok(projeto);
            }
        }
        public class ProjetoCreateDTO
        {
            public string NomeProjeto { get; set; } = null!;
            public string NomeCliente { get; set; } = null!;
            public decimal? PrecoHora { get; set; }
            public decimal IdUtilizador { get; set; }
        }
    }


   
    