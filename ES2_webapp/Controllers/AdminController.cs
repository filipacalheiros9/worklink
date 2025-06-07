using Microsoft.AspNetCore.Mvc;
using WebApplication2.Entities;
using WebApplication2.Services;
using System.Linq; // <- Importante para o Select()

namespace WebApplication2.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
    {
        private readonly IUtilizadorService _utilizadorService;
        private readonly IProjetoService _projetoService;

        public AdminController(IUtilizadorService utilizadorService, IProjetoService projetoService)
        {
            _utilizadorService = utilizadorService;
            _projetoService = projetoService;
        }

        [HttpGet("utilizadores")]
        public async Task<IActionResult> GetUtilizadores()
        {
            try
            {
                var utilizadores = await _utilizadorService.GetAllUtilizadoresAsync();
                var ordenados = utilizadores.OrderBy(u => u.IdUtilizador); // <- Corrigido aqui
                return Json(ordenados);
            }
            catch (Exception ex)
            {
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

        [HttpGet("projetos")]
        public async Task<IActionResult> GetProjetos()
        {
            var projetos = await _projetoService.GetAllWithCriadorAsync();

            var response = projetos.Select(p => new
            {
                idProjeto = p.IdProjeto,
                nomeProjeto = p.NomeProjeto,
                nomeCliente = p.NomeCliente,
                criador = new
                {
                    nome = p.Criador?.Nome ?? "N/A"
                }
            });

            return Json(response);
        }
    }
}
