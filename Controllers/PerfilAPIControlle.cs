using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PerfilApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PerfilApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPerfil(decimal id)
        {
            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound(new { Message = "Utilizador não encontrado." });
            }

            return Ok(utilizador);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePerfil(decimal id, [FromBody] UtilizadorUpdateDto model)
        {
            if (id != model.IdUtilizador)
            {
                return BadRequest(new { Message = "ID do utilizador não corresponde." });
            }

            var utilizador = await _context.Utilizadores.FindAsync(id);
            if (utilizador == null)
            {
                return NotFound(new { Message = "Utilizador não encontrado." });
            }

            utilizador.Nome = model.Nome;
            utilizador.Username = model.Username;
            utilizador.Password = model.Password;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Perfil atualizado com sucesso.", Utilizador = utilizador });
        }
    }
}