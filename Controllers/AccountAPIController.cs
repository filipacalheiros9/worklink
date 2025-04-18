using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Entities;

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
            return await _context.Utilizadores.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Utilizador>> GetUser(decimal id)
        {
            var user = await _context.Utilizadores.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Register.RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new Utilizador
                {
                    Nome = model.FullName,
                    Username = model.Username,
                    Password = model.Password,
                    cargo = model.cargo
                };

                _context.Utilizadores.Add(user);
                await _context.SaveChangesAsync();

                return Ok();
            }

            return BadRequest(ModelState);
        }
    }

    
    
    
    
    
}