using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;

namespace WebApplication2.Controllers;

public class PerfilController : Controller
{
    private readonly ApplicationDbContext _context;

    public PerfilController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePerfil([FromBody] UtilizadorUpdateDto model)
    {
        var idStr = HttpContext.Session.GetString("IdUtilizador");
        if (string.IsNullOrEmpty(idStr))
            return Unauthorized(new { Message = "Sessão inválida ou expirada." });

        decimal id = decimal.Parse(idStr);

        if (id != model.IdUtilizador)
            return BadRequest(new { Message = "ID da sessão não coincide com o pedido." });

        var utilizador = await _context.Utilizadores
            .Where(u => u.IdUtilizador == id)
            .FirstOrDefaultAsync();

        if (utilizador == null)
            return NotFound(new { Message = "Utilizador não encontrado." });

        try
        {
            utilizador.Nome = model.Nome;
            utilizador.Username = model.Username;
            utilizador.Password = model.Password;

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Perfil atualizado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao atualizar: " + ex.Message });
        }
    }




    
}