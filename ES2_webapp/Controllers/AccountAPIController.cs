using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using ES2_webapp.Data;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Entities;
using WebApplication2.Factories;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountAPIController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AccountAPIController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Utilizador>>> GetUsers()
        {
            // Inclui o Cargo para que ele esteja disponível ao consultar usuários
            return await _context.Utilizadores
                .Include(u => u.Cargo)
                .ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilizador>> GetUser(int id) // ou decimal se IdUtilizador for decimal
        {
            var user = await _context.Utilizadores
                .Include(u => u.Cargo)
                .FirstOrDefaultAsync(u => u.IdUtilizador == id);

            if (user == null)
                return NotFound();

            return user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register.RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var totalUtilizadores = await _context.Utilizadores.CountAsync();
            
            var novoUtilizador = UtilizadorFactory.CriarNovo(
                model.FullName,
                model.Username,
                model.Password,
                totalUtilizadores
            );

            _context.Utilizadores.Add(novoUtilizador);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
