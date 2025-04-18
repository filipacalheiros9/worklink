using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.DTO;
using WebApplication2.Entities;
using WebApplication2.Services;

namespace WebApplication2.Controllers;



public class PerfilController : ControllerBase
{
    private readonly IUtilizadorService _service;

    public PerfilController(IUtilizadorService service)
    {
        _service = service;
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePerfil([FromBody] UtilizadorUpdate model)
    {
        var idStr = HttpContext.Session.GetString("IdUtilizador");
        if (string.IsNullOrEmpty(idStr))
            return Unauthorized(new { Message = "Sessão inválida ou expirada." });

        decimal id = decimal.Parse(idStr);

        if (id != model.IdUtilizador)
            return BadRequest(new { Message = "ID da sessão não coincide com o pedido." });

        try
        {
            await _service.AtualizarPerfil(id, model);
            return Ok(new { Message = "Perfil atualizado com sucesso." });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { Message = "Erro ao atualizar: " + ex.Message });
        }
    }
}

