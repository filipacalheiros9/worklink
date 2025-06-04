// WebApplication2/Controllers/TarefasController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.Data;
using WebApplication2.DTO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("Tarefas")]
    public class TarefasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TarefasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ============================================================================
        // Index (renderiza a View de tarefas, recebendo somente o projetoId por URL)
        // ============================================================================
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(int projetoId)
        {
            var projeto = await _context.Projetos.FindAsync(projetoId);
            if (projeto == null) return NotFound();

            ViewBag.ProjetoId = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;
            return View("~/Views/Home/Tarefas.cshtml");
        }

        // ===================================================
        // MoverTarefa: só atualiza a coluna “Fase” da tarefa
        // ===================================================
        [HttpPut("MoverTarefa/{idTarefa}")]
        public async Task<IActionResult> MoverTarefa(int idTarefa, [FromBody] int novaFase)
        {
            var projetoTarefa = await _context.ProjetoTarefa
                .FirstOrDefaultAsync(pt => pt.TarefaId == idTarefa);

            if (projetoTarefa == null)
                return NotFound(new { message = "Tarefa não encontrada no projeto." });

            projetoTarefa.Fase = novaFase;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Fase atualizada com sucesso." });
        }

        // =======================================================================================
        // GetTarefasPorProjeto: lista as tarefas associadas, permitindo acesso a criador ou membro de equipa
        // =======================================================================================
        [HttpGet("MinhasTarefas/{idProjeto}")]
        public async Task<IActionResult> GetTarefasPorProjeto(int idProjeto)
        {
            // 1) Extrair o Id do utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            // 2) Obter o projeto (sem filtrar pelo criador)
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == idProjeto);

            if (projeto == null)
                return NotFound(new { message = "Projeto não existe." });

            var isCriador = projeto.IdUtilizador == idUtilizador;
            var isMembroEquipa = false;

            if (!isCriador && projeto.EquipaId.HasValue)
            {
                isMembroEquipa = await _context.EquipaUtilizadores
                    .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                 && eu.UtilizadorId == idUtilizador);
            }

            if (!isCriador && !isMembroEquipa)
                return Unauthorized(new { message = "Sem permissão para aceder a este projeto." });

            // 3) Buscar todas as tarefas associadas a esse projeto
            var tarefas = await _context.ProjetoTarefa
                .Where(pt => pt.IdProjeto == idProjeto)
                .Select(pt => new
                {
                    pt.Tarefa.IdTarefa,
                    pt.Tarefa.NomeTarefa,
                    pt.Tarefa.PrecoHora,
                    pt.Tarefa.DtInicio,
                    pt.Tarefa.HrInicio,
                    pt.Tarefa.DtFim,
                    pt.Tarefa.HrFim,
                    // Se houver responsável, mostrar também o nome
                    IdResponsavel = pt.Tarefa.IdUtilizador,
                    ResponsavelNome = pt.Tarefa.IdUtilizador != null
                        ? _context.Utilizadores
                            .Where(u => u.IdUtilizador == pt.Tarefa.IdUtilizador)
                            .Select(u => u.Nome)
                            .FirstOrDefault()
                        : null,
                    pt.Fase
                })
                .ToListAsync();

            return Ok(tarefas);
        }

        // =======================================================================================
        // CriarTarefa: cria nova tarefa associada a um projeto de equipa
        // ***************************************************************************************
        // Lê TarefaCreateEquipa (não TarefaCreate)
        // =======================================================================================
        [HttpPost("CriarTarefa")]
        public async Task<IActionResult> CriarTarefa([FromBody] TarefaCreateEquipa dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos." });

            // 1) Verificar claim do utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized(new { message = "Utilizador não autenticado." });

            // 2) Buscar o projeto onde vai ser criada a tarefa
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == dto.IdProjeto);

            if (projeto == null)
                return NotFound(new { message = "Projeto não existe." });

            // 3) Verificar se é criador OU membro da equipa
            var isCriador = projeto.IdUtilizador == idUtilizador;
            var isMembroEquipa = false;
            if (!isCriador && projeto.EquipaId.HasValue)
            {
                isMembroEquipa = await _context.EquipaUtilizadores
                    .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                 && eu.UtilizadorId == idUtilizador);
            }
            if (!isCriador && !isMembroEquipa)
                return Unauthorized(new { message = "Sem permissão para criar tarefa neste projeto." });

            // 4) Decidir quem será o responsável pela tarefa:
            int? responsavelId = null;
            if (dto.IdUtilizadorResponsavel.HasValue)
            {
                // Se o projeto tem equipa, certifica-se de que este responsável é membro dela
                if (projeto.EquipaId.HasValue)
                {
                    var ehMembro = await _context.EquipaUtilizadores
                        .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                     && eu.UtilizadorId == dto.IdUtilizadorResponsavel.Value);
                    if (!ehMembro)
                        return BadRequest(new { message = "Responsável indicado não faz parte desta equipa." });
                }
                responsavelId = dto.IdUtilizadorResponsavel.Value;
            }
            else
            {
                // Nenhum responsável explicitamente enviado: deixamos como null
                responsavelId = null;
            }

            // 5) Criar e guardar a entidade Tarefa
            var tarefa = new Tarefa
            {
                NomeTarefa   = dto.NomeTarefa,
                DtInicio     = dto.DtInicio,
                HrInicio     = dto.HrInicio,
                DtFim        = dto.DtFim,
                HrFim        = dto.HrFim,
                PrecoHora    = dto.PrecoHora,
                IdUtilizador = responsavelId    // pode ser null
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            // 6) Associar ao ProjetoTarefa
            var projetoTarefa = new ProjetoTarefa
            {
                IdProjeto = dto.IdProjeto,
                TarefaId  = tarefa.IdTarefa,
                Fase      = 0 // ou valor inicial que preferir
            };
            _context.ProjetoTarefa.Add(projetoTarefa);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message    = "Tarefa criada com sucesso.",
                idTarefa   = tarefa.IdTarefa,
                nomeTarefa = tarefa.NomeTarefa,
                precoHora  = tarefa.PrecoHora,
                dtInicio   = tarefa.DtInicio
            });
        }

        // =======================================================================================
        // AtualizarTarefa: atualiza campos básicos da tarefa e opcionalmente altera responsável
        // =======================================================================================
        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaEditDto dto)
        {
            try
            {
                // 1) Buscar a tarefa
                var tarefa = await _context.Tarefas.FindAsync(id);
                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada." });

                // 2) Verificar permissão:
                var projetoTarefa = await _context.ProjetoTarefa
                    .FirstOrDefaultAsync(pt => pt.TarefaId == id);

                if (projetoTarefa != null)
                {
                    var projeto = await _context.Projetos
                        .FirstOrDefaultAsync(p => p.IdProjeto == projetoTarefa.IdProjeto);
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                    if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                        return Unauthorized();

                    bool isCriador = projeto.IdUtilizador == idUtilizador;
                    bool isMembroEquipa = false;
                    if (!isCriador && projeto.EquipaId.HasValue)
                    {
                        isMembroEquipa = await _context.EquipaUtilizadores
                            .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                         && eu.UtilizadorId == idUtilizador);
                    }
                    bool isResponsavelAtual = tarefa.IdUtilizador == idUtilizador;

                    if (!isCriador && !isMembroEquipa && !isResponsavelAtual)
                        return Unauthorized(new { message = "Sem permissão para editar esta tarefa." });
                }

                // 3) Atualizar campos básicos
                tarefa.NomeTarefa = dto.NomeTarefa;
                tarefa.DtFim      = dto.DtFim ?? tarefa.DtFim;
                tarefa.PrecoHora  = dto.PrecoHora;

                // 4) Se vier IdUtilizadorResponsavel, validamos e alteramos
                if (dto.IdUtilizadorResponsavel.HasValue)
                {
                    int novoResp = dto.IdUtilizadorResponsavel.Value;

                    if (projetoTarefa != null)
                    {
                        var projeto = await _context.Projetos
                            .FirstOrDefaultAsync(p => p.IdProjeto == projetoTarefa.IdProjeto);
                        if (projeto.EquipaId.HasValue)
                        {
                            var ehMembro = await _context.EquipaUtilizadores
                                .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                             && eu.UtilizadorId == novoResp);
                            if (!ehMembro)
                                return BadRequest(new { message = "Novo responsável não faz parte desta equipa." });
                        }
                    }

                    tarefa.IdUtilizador = novoResp;
                }

                await _context.SaveChangesAsync();
                return Ok(new { message = "Tarefa atualizada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno ao atualizar a tarefa.",
                    detalhe = ex.Message
                });
            }
        }

        // =======================================================================================
        // Exclui o projeto e todas as tarefas associadas (método existente, mantido sem alterações)
        // =======================================================================================
        [HttpDelete("EliminarProjeto/{id}")]
        public async Task<IActionResult> EliminarProjeto(int id)
        {
            try
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return Unauthorized();

                var projeto = await _context.Projetos
                    .Include(p => p.ProjetosTarefas)
                    .ThenInclude(pt => pt.Tarefa)
                    .FirstOrDefaultAsync(p => p.IdProjeto == id && p.IdUtilizador == idUtilizador);

                if (projeto == null)
                    return NotFound(new { message = "Projeto não encontrado ou não pertence ao utilizador." });

                _context.Projetos.Remove(projeto);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Projeto e tarefas associadas eliminados com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro ao eliminar projeto", detalhe = ex.Message });
            }
        }

        // =======================================================================================
        // Novo endpoint: retorna todos os membros da equipa de um projeto (para popular dropdown)
        // =======================================================================================
        [HttpGet("MembrosEquipe/{idProjeto}")]
        public async Task<IActionResult> GetMembrosEquipe(int idProjeto)
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == idProjeto);
            if (projeto == null)
                return NotFound(new { message = "Projeto não existe." });

            bool isCriador = projeto.IdUtilizador == idUtilizador;
            bool isMembroEquipa = false;
            if (!isCriador && projeto.EquipaId.HasValue)
            {
                isMembroEquipa = await _context.EquipaUtilizadores
                    .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                 && eu.UtilizadorId == idUtilizador);
            }
            if (!isCriador && !isMembroEquipa)
                return Forbid();

            IEnumerable<object> membros;
            if (projeto.EquipaId.HasValue)
            {
                membros = await _context.EquipaUtilizadores
                    .Where(eu => eu.EquipaId == projeto.EquipaId)
                    .Include(eu => eu.Utilizador)
                    .Select(eu => new
                    {
                        idUtilizador = eu.UtilizadorId,
                        nome = eu.Utilizador.Nome // Proprietário do Utilizador deve ter a propriedade Nome
                    })
                    .OrderBy(x => x.nome)
                    .ToListAsync();
            }
            else
            {
                // Projeto pessoal: único "membro" é o próprio criador
                var criador = await _context.Utilizadores
                    .FirstOrDefaultAsync(u => u.IdUtilizador == projeto.IdUtilizador);
                if (criador != null)
                    membros = new[] { new { idUtilizador = criador.IdUtilizador, nome = criador.Nome } };
                else
                    membros = Array.Empty<object>();
            }

            return Ok(membros);
        }
    }
}
