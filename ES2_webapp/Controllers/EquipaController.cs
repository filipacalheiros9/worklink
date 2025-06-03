using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.Data;
using WebApplication2.DTO;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EquipaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EquipaController(ApplicationDbContext context)
        {
            _context = context;
        }
        

        [HttpPost("Criar")]
        public async Task<IActionResult> CriarEquipa([FromBody] CriarEquipaDto dto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userIdClaim, out var idCriador))
                return Unauthorized(new { message = "Utilizador não autenticado." });

            if (string.IsNullOrWhiteSpace(dto.Nome))
                return BadRequest(new { message = "O nome da equipa é obrigatório." });

            var equipa = new Equipa { Nome = dto.Nome, IdCriador = idCriador };
            _context.Equipas.Add(equipa);
            await _context.SaveChangesAsync();

            _context.EquipaUtilizadores.Add(new EquipaUtilizador { EquipaId = equipa.IdEquipa, UtilizadorId = idCriador });
            await _context.SaveChangesAsync();

            return Ok(new { message = "Equipa criada com sucesso.", equipaId = equipa.IdEquipa });
        }

        [HttpGet("Minhas")]
        public async Task<IActionResult> MinhasEquipas()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var equipas = await _context.EquipaUtilizadores
                .Where(eu => eu.UtilizadorId == userId)
                .Include(eu => eu.Equipa)
                .Select(eu => new {
                    eu.Equipa.IdEquipa,
                    eu.Equipa.Nome,
                    IsCriador = eu.Equipa.IdCriador == userId
                })
                .ToListAsync();

            return Ok(equipas);
        }

        [HttpPost("Convidar")]
        public async Task<IActionResult> Convidar([FromBody] EnviarConviteDto dto)
        {
            var remetenteId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(remetenteId, out var idRemetente))
                return Unauthorized();

            var equipa = await _context.Equipas.FirstOrDefaultAsync(e => e.IdEquipa == dto.IdEquipa);
            if (equipa == null)
                return NotFound(new { message = "Equipa não encontrada." });

            if (equipa.IdCriador != idRemetente)
                return Forbid("Apenas o criador da equipa pode convidar utilizadores.");

            var destinatario = await _context.Utilizadores
                .FirstOrDefaultAsync(u => u.Username == dto.UsernameDestinatario);

            if (destinatario == null)
                return NotFound(new { message = "Utilizador não encontrado." });

            var convite = new Convite
            {
                Mensagem = dto.Mensagem ?? "",
                FoiLido = false,
                Resposta = null,
                IdUtilizadorRemetente = idRemetente,
                IdUtilizadorDestinatario = destinatario.IdUtilizador,
                DataEnvio = DateTime.UtcNow,
                IdEquipa = dto.IdEquipa
            };

            _context.Convites.Add(convite);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Convite enviado com sucesso." });
        }

        [HttpGet("PesquisarUtilizador")]
        public async Task<IActionResult> PesquisarUtilizador([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
                return BadRequest(new { message = "Insira pelo menos 3 letras." });

            var resultados = await _context.Utilizadores
                .Where(u => u.Username.ToLower().Contains(query.ToLower()))
                .Select(u => new { u.Username })
                .Take(5)
                .ToListAsync();

            return Ok(resultados);
        }

        [HttpDelete("Apagar/{id}")]
        public async Task<IActionResult> ApagarEquipa(int id)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized();

            var equipa = await _context.Equipas
                .Include(e => e.Convites)
                .Include(e => e.EquipaUtilizadores)
                .Include(e => e.Projetos)
                .ThenInclude(p => p.ProjetosTarefas)
                .ThenInclude(pt => pt.Tarefa)
                .FirstOrDefaultAsync(e => e.IdEquipa == id && e.IdCriador == userId);

            if (equipa == null)
                return NotFound(new { message = "Equipa não encontrada ou não pertence ao utilizador." });

            _context.Convites.RemoveRange(equipa.Convites);
            _context.EquipaUtilizadores.RemoveRange(equipa.EquipaUtilizadores);

            foreach (var projeto in equipa.Projetos)
            {
                foreach (var pt in projeto.ProjetosTarefas)
                    _context.Tarefas.Remove(pt.Tarefa);

                _context.ProjetoTarefa.RemoveRange(projeto.ProjetosTarefas);
                _context.Projetos.Remove(projeto);
            }

            _context.Equipas.Remove(equipa);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Equipa e dependências eliminadas com sucesso." });
        }

        [HttpGet("GetMembros")]
        public async Task<IActionResult> GetMembros(int id)
        {
            var equipa = await _context.Equipas
                .Include(e => e.EquipaUtilizadores)
                .ThenInclude(eu => eu.Utilizador)
                .FirstOrDefaultAsync(e => e.IdEquipa == id);

            if (equipa == null)
                return NotFound();

            var membros = equipa.EquipaUtilizadores.Select(eu => new {
                id = eu.Utilizador.IdUtilizador,
                nome = eu.Utilizador.Nome
            });

            return Ok(membros);
        }

        [HttpPost("RemoverMembro")]
        public async Task<IActionResult> RemoverMembro([FromBody] RemoverMembroDto dto)
        {
            var equipa = await _context.Equipas
                .Include(e => e.EquipaUtilizadores)
                .FirstOrDefaultAsync(e => e.IdEquipa == dto.EquipaId);

            if (equipa == null)
                return NotFound();

            var claimId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!int.TryParse(claimId, out var userId))
                return Unauthorized();

            var isDono = equipa.IdCriador == userId;
            var isSelf = userId == dto.UserId;

            // ❌ Só permitir:
            // - Dono remover outros
            // - Membro remover-se a si próprio
            if (!isDono && !isSelf)
                return Forbid();

            // ❌ Dono não se pode remover a si mesmo
            if (isDono && isSelf)
                return BadRequest(new { message = "O criador da equipa não pode sair da própria equipa." });

            var membro = equipa.EquipaUtilizadores.FirstOrDefault(m => m.UtilizadorId == dto.UserId);
            if (membro != null)
            {
                _context.EquipaUtilizadores.Remove(membro);
                await _context.SaveChangesAsync();
                return Ok(new { message = isSelf ? "Saíste da equipa." : "Membro removido." });
            }

            return NotFound(new { message = "Membro não encontrado na equipa." });
        }

        [HttpGet("Convites")]
        public async Task<IActionResult> ObterConvitesDropdown()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userId, out var idUser))
                return Unauthorized();

            var convites = await _context.Convites
                .Include(c => c.Equipa)
                .Where(c => c.IdUtilizadorDestinatario == idUser)
                .OrderByDescending(c => c.DataEnvio)
                .Take(3)
                .ToListAsync();

            return Ok(convites.Select(c => new {
                c.IdMensagem,
                Equipa = c.Equipa?.Nome,
                c.Mensagem,
                Data = c.DataEnvio.ToString("dd/MM/yyyy HH:mm"),
                c.Resposta
            }));
        }

        [HttpGet("ConvitesResumo")]
        public async Task<IActionResult> ConvitesResumo()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userId, out var idUser))
                return Unauthorized();

            var convites = await _context.Convites
                .Include(c => c.Equipa)
                .Where(c => c.IdUtilizadorDestinatario == idUser)
                .OrderByDescending(c => c.DataEnvio)
                .Take(3)
                .Select(c => new {
                    equipaNome = c.Equipa.Nome,
                    mensagem = c.Mensagem
                })
                .ToListAsync();

            return Ok(convites);
        }

        [HttpPost("ResponderConvite/{idMensagem}")]
        public async Task<IActionResult> ResponderConvite(int idMensagem, [FromQuery] bool aceitar)
        {
            var convite = await _context.Convites.FindAsync(idMensagem);
            if (convite == null) return NotFound();

            convite.Resposta = aceitar;
            convite.FoiLido = true;

            if (aceitar)
            {
                _context.EquipaUtilizadores.Add(new EquipaUtilizador
                {
                    EquipaId = convite.IdEquipa,
                    UtilizadorId = convite.IdUtilizadorDestinatario
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = aceitar ? "Convite aceite." : "Convite recusado." });
        }

        [HttpGet("/Equipa/Convites")]
        public async Task<IActionResult> ConvitesPage()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;
            if (!decimal.TryParse(userId, out var idUser))
                return RedirectToAction("Login", "Home");

            var convites = await _context.Convites
                .Include(c => c.Equipa)
                .Where(c => c.IdUtilizadorDestinatario == idUser)
                .OrderByDescending(c => c.DataEnvio)
                .ToListAsync();

            return View("~/Views/Equipas/Convite.cshtml", convites);
        }
    }
}
