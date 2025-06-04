using ES2_webapp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;
using WebApplication2.DTO;
using WebApplication2.Services;
using System;
using System.Linq;    // para LINQ
using System.Collections.Generic; // para IEnumerable

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly IProjetoService _projetoService;

        public ProjetoController(IProjetoService projetoService)
        {
            _projetoService = projetoService;
        }

        [HttpGet("ProjetosPessoais")]
        public IActionResult GetProjetosPessoais()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            var projetos = _projetoService.ObterProjetosPessoais(idUtilizador);
            return Ok(projetos);
        }

        [HttpGet("ProjetosEquipa")]
        public IActionResult GetProjetosEquipa()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            var projetos = _projetoService.ObterProjetosEquipa(idUtilizador);
            return Ok(projetos);
        }

        [HttpPost("CriarProjeto")]
        public async Task<IActionResult> CriarProjeto([FromBody] ProjetoCreate projetoDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "Dados inválidos." });

                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return BadRequest(new { message = "ID do utilizador inválido." });

                var projeto = new Projeto
                {
                    NomeProjeto = projetoDTO.NomeProjeto,
                    NomeCliente = projetoDTO.NomeCliente,
                    IdUtilizador = idUtilizador
                };

                await _projetoService.AddProjetoAsync(projeto);

                return Ok(new
                {
                    nomeProjeto = projeto.NomeProjeto,
                    nomeCliente = projeto.NomeCliente,
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno",
                    detalhe = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpPut("AtualizarProjeto/{id}")]
        public async Task<IActionResult> AtualizarProjeto(decimal id, [FromBody] ProjetoCreate projetoDTO)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return Unauthorized();

                var projeto = await _projetoService.GetProjetoByIdAsync((int)id);
                if (projeto == null || projeto.IdUtilizador != idUtilizador)
                    return NotFound(new { message = "Projeto não encontrado ou não pertence a este utilizador." });

                projeto.NomeProjeto = projetoDTO.NomeProjeto;
                projeto.NomeCliente = projetoDTO.NomeCliente;

                await _projetoService.UpdateProjetoAsync(projeto);

                return Ok(new
                {
                    mensagem = "Projeto atualizado com sucesso!",
                    projetoAtualizado = new
                    {
                        projeto.IdProjeto,
                        projeto.NomeProjeto,
                        projeto.NomeCliente,
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno", detalhe = ex.Message });
            }
        }

        [HttpDelete("EliminarProjeto/{id}")]
        public async Task<IActionResult> EliminarProjeto(decimal id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return Unauthorized();

                var projeto = await _projetoService.GetProjetoByIdAsync((int)id);
                if (projeto == null || projeto.IdUtilizador != idUtilizador)
                    return NotFound(new { message = "Projeto não encontrado ou não te pertence." });

                await _projetoService.DeleteProjetoAsync((int)id);

                return Ok(new { message = "Projeto eliminado com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno",
                    detalhe = ex.InnerException?.Message ?? ex.Message
                });
            }
        }
        
        [HttpGet("Autocomplete")]
        public IActionResult Autocomplete(string term)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();
            
            IEnumerable<Projeto> projetosPessoais = _projetoService.ObterProjetosPessoais(idUtilizador);
            IEnumerable<Projeto> projetosEquipa   = _projetoService.ObterProjetosEquipa(idUtilizador);

            var resultados = projetosPessoais
                .Concat(projetosEquipa)
                .Where(p => !string.IsNullOrWhiteSpace(p.NomeProjeto)
                            && p.NomeProjeto
                                 .IndexOf(term ?? string.Empty,
                                          StringComparison.OrdinalIgnoreCase) >= 0)
                .GroupBy(p => p.IdProjeto) // evita duplicados, se houver
                .Select(g => g.First())
                .Select(p => new
                {
                    idProjeto   = p.IdProjeto,
                    nomeProjeto = p.NomeProjeto
                })
                .OrderBy(x => x.nomeProjeto)
                .Take(10)
                .ToList();

            return Ok(resultados);
        }
    }
}
