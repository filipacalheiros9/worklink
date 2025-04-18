using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;
using WebApplication2.Models;


namespace WebApplication2.Controllers
{
    public class ProjetoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjetoController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> CriarProjeto([FromBody] ProjetoCreate projetoDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Dados inválidos." });
            }

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Utilizador não autenticado." });
            }

            decimal idUtilizador = decimal.Parse(userIdClaim.Value);

            var projeto = new Projeto
            {
                NomeProjeto = projetoDTO.NomeProjeto,
                NomeCliente = projetoDTO.NomeCliente,
                PrecoHora = projetoDTO.PrecoHora,
                IdUtilizador = idUtilizador
            };

            _context.Projetos.Add(projeto);
            await _context.SaveChangesAsync();

            return Ok(new 
            {
                nomeProjeto = projeto.NomeProjeto,
                nomeCliente = projeto.NomeCliente,
                precoHora = projeto.PrecoHora
            });
        }
    }

    public class ProjetoCreate
    {
        public string NomeProjeto { get; set; }
        public string NomeCliente { get; set; }
        public decimal? PrecoHora { get; set; }
    }
}