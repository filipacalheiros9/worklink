using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Services;

namespace WebApplication2.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IUtilizadorService _utilizadorService;

        public AdminController(IUtilizadorService utilizadorService)
        {
            _utilizadorService = utilizadorService;
        }

        [HttpGet("utilizadores")]
        public async Task<IActionResult> GetUtilizadores()
        {
            try
            {
                var utilizadores = await _utilizadorService.GetAllUtilizadoresAsync();
                return Json(utilizadores);
            }
            catch (Exception ex)
            {
                // Retorna o erro para o front-end com status 500
                return StatusCode(500, $"Erro interno: {ex.Message}");
            }
        }


        [HttpPost("utilizadores")]
        public async Task<IActionResult> CriarUtilizador([FromBody] Utilizador utilizador)
        {
            await _utilizadorService.RegisterUtilizadorAsync(utilizador);
            return Ok();
        }

        [HttpPut("utilizadores/{id}")]
        public async Task<IActionResult> AtualizarUtilizador(decimal id, [FromBody] Utilizador model)
        {
            await _utilizadorService.AtualizarPerfil(id, new DTO.UtilizadorUpdate
            {
                Nome = model.Nome,
                Username = model.Username,
                Password = model.Password
            });

            return Ok();
        }

        [HttpDelete("utilizadores/{id}")]
        public async Task<IActionResult> EliminarUtilizador(decimal id)
        {
            await _utilizadorService.DeleteUtilizadorAsync(id);
            return Ok();
        }
    }
}