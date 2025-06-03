using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.Data;         // namespace do seu DbContext
using WebApplication2.DTO;     // onde deixar CriarProjetoDto, ProjetosEquipasViewModel, etc.
using WebApplication2.Entities; // onde está a entidade Projeto, Equipa, etc.

namespace WebApplication2.Controllers
{
    // Rota raiz: /ProjetoEquipas/...
    [Route("ProjetoEquipas")]
    public class ProjetoEquipasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjetoEquipasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /ProjetoEquipas/ProjetosPorEquipa?idEquipa={id}
        [HttpGet("ProjetosPorEquipa")]
        public async Task<IActionResult> ProjetosPorEquipa(int idEquipa)
        {
            // Verifica se o user está autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var userId))
                return RedirectToAction("Login", "Home");

            // Carrega a equipa e seus projetos do BD
            var equipa = await _context.Equipas
                .Include(e => e.Projetos)
                    .ThenInclude(p => p.ProjetosTarefas)
                        .ThenInclude(pt => pt.Tarefa)
                .FirstOrDefaultAsync(e => e.IdEquipa == idEquipa);

            if (equipa == null)
                return NotFound($"Equipa com Id = {idEquipa} não encontrada.");

            // Mapeia para um ViewModel simplificado
            var listaProjetos = equipa.Projetos
                .Select(p => new ProjetosEquipasViewModel
                {
                    IdProjeto    = p.IdProjeto,
                    NomeProjeto  = p.NomeProjeto,
                    NomeCliente  = p.NomeCliente,
                    IdUtilizador = p.IdUtilizador,
                    EquipaId     = p.EquipaId ?? 0
                })
                .ToList();

            // Retorna a View Razor que você deve criar em /Views/Equipas/ProjetosEquipas.cshtml
            return View("~/Views/Equipas/ProjetosEquipas.cshtml", listaProjetos);
        }

        // POST /ProjetoEquipas/CriarProjetoEquipa
        [HttpPost("CriarProjetoEquipa")]
        public async Task<IActionResult> CriarProjetoEquipa([FromBody] CriarProjetoDto dto)
        {
            // 1) Verificar autenticação
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var userId))
                return Unauthorized(); // 401

            // 2) Verificar pertença à equipa
            var membro = await _context.EquipaUtilizadores
                .AnyAsync(eu => eu.EquipaId == dto.EquipaId && eu.UtilizadorId == userId);
            if (!membro)
                return Forbid("Não pertences a esta equipa.");

            // 3) Criar objeto Projeto e gravar
            var projeto = new Projeto
            {
                NomeProjeto = dto.Nome,
                NomeCliente  = dto.Cliente,
                IdUtilizador = userId,
                EquipaId     = dto.EquipaId
            };

            _context.Projetos.Add(projeto);
            await _context.SaveChangesAsync();

            return Ok(new { projeto.IdProjeto, projeto.NomeProjeto });
        }

        // GET /ProjetoEquipas/PorEquipa/{equipaId}
        [HttpGet("PorEquipa/{equipaId}")]
        public async Task<IActionResult> ListarProjetosPorEquipa(int equipaId)
        {
            // 1) Verificar autenticação
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var userId))
                return Unauthorized(); // 401

            // 2) Verificar pertença à equipa
            var membro = await _context.EquipaUtilizadores
                .AnyAsync(eu => eu.EquipaId == equipaId && eu.UtilizadorId == userId);
            if (!membro)
                return Forbid("Não pertences a esta equipa.");

            // 3) Buscar projetos e incluir username do criador
            var projetos = await _context.Projetos
                .Include(p => p.Criador)
                .Where(p => p.EquipaId == equipaId)
                .Select(p => new
                {
                    p.IdProjeto,
                    NomeProjeto = p.NomeProjeto,
                    NomeCliente = p.NomeCliente,
                    Criador     = p.Criador.Username
                })
                .ToListAsync();

            return Ok(projetos);
        }

        // PUT /ProjetoEquipas/AtualizarProjetoEquipa/{id}
        [HttpPut("AtualizarProjetoEquipa/{id}")]
        public async Task<IActionResult> AtualizarProjetoEquipa(int id, [FromBody] ProjetoUpdateDto dto)
        {
            // 1) Verificar autenticação
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var userId))
                return Unauthorized(); // 401

            // 2) Encontrar o projeto
            var projeto = await _context.Projetos.FirstOrDefaultAsync(p => p.IdProjeto == id);
            if (projeto == null)
                return NotFound($"Projeto com Id = {id} não encontrado.");

            // 3) Verificar se é criador
            if (projeto.IdUtilizador != userId)
                return Forbid("Não és o criador deste projeto.");

            // 4) Atualizar campos
            projeto.NomeProjeto = dto.NomeProjeto;
            projeto.NomeCliente = dto.NomeCliente;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Projeto atualizado com sucesso." });
        }

        // DELETE /ProjetoEquipas/EliminarProjetoEquipa/{id}
        [HttpDelete("EliminarProjetoEquipa/{id}")]
        public async Task<IActionResult> EliminarProjetoEquipa(int id)
        {
            // 1) Verificar autenticação
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var userId))
                return Unauthorized(); // 401

            // 2) Buscar projeto com tarefas associadas
            var projeto = await _context.Projetos
                .Include(p => p.ProjetosTarefas)
                    .ThenInclude(pt => pt.Tarefa)
                .FirstOrDefaultAsync(p => p.IdProjeto == id);
            if (projeto == null)
                return NotFound($"Projeto com Id = {id} não encontrado.");

            // 3) Verificar se é criador
            if (projeto.IdUtilizador != userId)
                return Forbid("Não és o criador deste projeto.");

            // 4) Remover tarefas filhas
            foreach (var pt in projeto.ProjetosTarefas)
                _context.Tarefas.Remove(pt.Tarefa);

            _context.ProjetoTarefa.RemoveRange(projeto.ProjetosTarefas);
            _context.Projetos.Remove(projeto);

            await _context.SaveChangesAsync();
            return Ok(new { message = "Projeto eliminado com sucesso." });
        }
    }
}
