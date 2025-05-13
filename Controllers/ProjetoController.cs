using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Entities;
using WebApplication2.DTO;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjetoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProjetoController(ApplicationDbContext context)
        {
            _context = context;
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

                // Verificar se o utilizador existe
                var utilizadorExiste = await _context.Utilizadores.AnyAsync(u => u.IdUtilizador == idUtilizador);
                if (!utilizadorExiste)
                    return NotFound(new { message = "Utilizador não existe na base de dados." });

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
            catch (Exception ex)
            {
                Console.WriteLine("🟥 ERRO INTERNO CATCH:");
                Console.WriteLine(ex.ToString());

                return StatusCode(500, new
                {
                    message = "Erro interno",
                    detalhe = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("MeusProjetos")]
        public async Task<IActionResult> GetMeusProjetos()
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return BadRequest(new { message = "ID do utilizador inválido." });

                var projetos = await _context.Projetos
                    .Where(p => p.IdUtilizador == idUtilizador)
                    .Select(p => new
                    {
                        p.IdProjeto,
                        p.NomeProjeto,
                        p.NomeCliente,
                        p.PrecoHora
                    })
                    .ToListAsync();

                return Ok(projetos);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO LISTAR PROJETOS:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Erro interno", detalhe = ex.Message });
            }
        }
        
        [HttpPut("AtualizarProjeto/{id}")]
        public async Task<IActionResult> AtualizarProjeto(decimal id, [FromBody] ProjetoCreate projetoDTO)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return BadRequest(new { message = "ID do utilizador inválido." });

                var projeto = await _context.Projetos
                    .FirstOrDefaultAsync(p => p.IdProjeto == id && p.IdUtilizador == idUtilizador);

                if (projeto == null)
                    return NotFound(new { message = "Projeto não encontrado ou não pertence a este utilizador." });

                
                projeto.NomeProjeto = projetoDTO.NomeProjeto;
                projeto.NomeCliente = projetoDTO.NomeCliente;
                projeto.PrecoHora = projetoDTO.PrecoHora;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensagem = "Projeto atualizado com sucesso!",
                    projetoAtualizado = new
                    {
                        projeto.IdProjeto,
                        projeto.NomeProjeto,
                        projeto.NomeCliente,
                        projeto.PrecoHora
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(" ERRO AO ATUALIZAR PROJETO:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Erro interno", detalhe = ex.Message });
            }
        }

        [HttpDelete("EliminarProjeto/{id}")]
        public async Task<IActionResult> EliminarProjeto(decimal id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null)
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                if (!decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return BadRequest(new { message = "ID do utilizador inválido." });

                var projeto = await _context.Projetos
                    .FirstOrDefaultAsync(p => p.IdProjeto == id && p.IdUtilizador == idUtilizador);

                if (projeto == null)
                    return NotFound(new { message = "Projeto não encontrado ou não te pertence." });

                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Projeto eliminado com sucesso." });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERRO AO ELIMINAR PROJETO:");
                Console.WriteLine(ex.ToString());
                return StatusCode(500, new { message = "Erro interno", detalhe = ex.Message });
            }
        }
    }
}
