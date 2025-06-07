using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.Data;
using WebApplication2.DTO;
using WebApplication2.Models;
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

        // ----------------------------------------------
        // View de Kanban: exibe a página de Tarefas para um dado projeto
        // ----------------------------------------------
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> Index(int projetoId)
        {
            var projeto = await _context.Projetos.FindAsync(projetoId);
            if (projeto == null)
                return NotFound();

            ViewBag.ProjetoId   = projeto.IdProjeto;
            ViewBag.NomeProjeto = projeto.NomeProjeto;
            ViewBag.EquipaId    = projeto.EquipaId; // pode ser null se for projeto individual

            return View("~/Views/Home/Tarefas.cshtml");
        }

        // ----------------------------------------------
        // MoverTarefa: altera a fase (coluna) de uma tarefa no Kanban
        // ----------------------------------------------
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

        // ----------------------------------------------
        // MinhasTarefas: retorna todas as tarefas de um projeto,
        // incluindo nome do responsável
        // ----------------------------------------------
        [HttpGet("MinhasTarefas/{idProjeto}")]
        public async Task<IActionResult> GetTarefasPorProjeto(int idProjeto)
        {
            // 1) Verificar se o utilizador autenticado existe
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            // 2) Verificar se o projeto existe
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == idProjeto);

            if (projeto == null)
                return NotFound(new { message = "Projeto não existe." });

            // 3) Verificar permissões: criador ou membro de equipa
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

            // 4) Buscar todas as tarefas associadas a esse projeto
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

        // ----------------------------------------------
        // **NOVO**: Retorna todas as tarefas de todos os projetos ligados
        // a uma equipa, para que qualquer membro da equipa veja tudo
        // ----------------------------------------------
        [HttpGet("TarefasEquipa/{idEquipa}")]
        public async Task<IActionResult> GetTarefasPorEquipa(int idEquipa)
        {
            // 1) Verificar utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized();

            // 2) Confirmar que este utilizador pertence à Equipa (ou é criador de algum projeto dessa equipa)
            var pertenceEquipa = await _context.EquipaUtilizadores
                .AnyAsync(eu => eu.EquipaId == idEquipa && eu.UtilizadorId == idUtilizador);

            if (!pertenceEquipa)
            {
                // Alternativa: permitir se for criador de algum projeto que tenha EquipaId = idEquipa
                var eCriadorProjeto = await _context.Projetos
                    .AnyAsync(p => p.EquipaId == idEquipa && p.IdUtilizador == idUtilizador);

                if (!eCriadorProjeto)
                    return Unauthorized(new { message = "Sem permissão para ver tarefas desta equipa." });
            }

            // 3) Trazer as tarefas de todos os projetos cujo Projeto.EquipaId == idEquipa
            var tarefasEquipa = await _context.ProjetoTarefa
                .Where(pt => pt.Projeto.EquipaId == idEquipa)
                .Select(pt => new
                {
                    pt.Tarefa.IdTarefa,
                    pt.Tarefa.NomeTarefa,
                    pt.Tarefa.PrecoHora,
                    pt.Tarefa.DtInicio,
                    pt.Tarefa.HrInicio,
                    pt.Tarefa.DtFim,
                    pt.Tarefa.HrFim,
                    IdResponsavel = pt.Tarefa.IdUtilizador,
                    ResponsavelNome = pt.Tarefa.IdUtilizador != null
                        ? _context.Utilizadores
                            .Where(u => u.IdUtilizador == pt.Tarefa.IdUtilizador)
                            .Select(u => u.Nome)
                            .FirstOrDefault()
                        : null,
                    pt.Fase,
                    NomeProjeto = pt.Projeto.NomeProjeto,
                    IdProjeto = pt.Projeto.IdProjeto
                })
                .ToListAsync();

            return Ok(tarefasEquipa);
        }

        // ----------------------------------------------
        // CriarTarefa: cria uma nova tarefa (eventualmente associando um responsável)
        // ----------------------------------------------
        [HttpPost("CriarTarefa")]
        public async Task<IActionResult> CriarTarefa([FromBody] TarefaCreateEquipa dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { message = "Dados inválidos." });

            // 1) Verificar claim do utilizador autenticado
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
            if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                return Unauthorized(new { message = "Utilizador não autenticado." });

            // 2) Buscar o projeto onde será criada a tarefa
            var projeto = await _context.Projetos
                .FirstOrDefaultAsync(p => p.IdProjeto == dto.IdProjeto);

            if (projeto == null)
                return NotFound(new { message = "Projeto não existe." });

            // 3) Verificar permissões: criador ou membro de equipa
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

            // 4) Se foi indicado um responsável, verificar se faz parte da equipa
            int? responsavelId = null;
            if (dto.IdUtilizadorResponsavel.HasValue)
            {
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

            // 5) Construir a entidade Tarefa e salvar
            var tarefa = new Tarefa
            {
                NomeTarefa   = dto.NomeTarefa,
                DtInicio     = dto.DtInicio,
                HrInicio     = dto.HrInicio,
                DtFim        = dto.DtFim,
                HrFim        = dto.HrFim,
                PrecoHora    = dto.PrecoHora,
                IdUtilizador = responsavelId
            };

            _context.Tarefas.Add(tarefa);
            await _context.SaveChangesAsync();

            // 6) Associar a ProjetoTarefa (inicialmente na fase 0)
            var projetoTarefa = new ProjetoTarefa
            {
                IdProjeto = dto.IdProjeto,
                TarefaId  = tarefa.IdTarefa,
                Fase      = 0
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

        // ----------------------------------------------
        // AtualizarTarefa: edita os campos de uma tarefa existente
        // ----------------------------------------------
        [HttpPut("AtualizarTarefa/{id}")]
        public async Task<IActionResult> AtualizarTarefa(int id, [FromBody] TarefaEditDto dto)
        {
            try
            {
                var tarefa = await _context.Tarefas.FindAsync(id);
                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada." });

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

                // Atualizar campos básicos
                tarefa.NomeTarefa = dto.NomeTarefa;
                if (dto.DtFim.HasValue)
                    tarefa.DtFim = dto.DtFim.Value;
                tarefa.PrecoHora = dto.PrecoHora;

                // Se indicaram novo responsável, verificar se faz parte da equipa
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

        // ----------------------------------------------
        // EliminarProjeto: apaga um projeto e todas as tarefas associadas
        // ----------------------------------------------
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

        // ----------------------------------------------
        // EliminarTarefa: apaga uma única tarefa (DELETE /Tarefas/EliminarTarefa/{id})
        // ----------------------------------------------
        [HttpDelete("EliminarTarefa/{id}")]
        public async Task<IActionResult> EliminarTarefa(int id)
        {
            try
            {
                // 1) Verificar claim do utilizador autenticado
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "id");
                if (userIdClaim == null || !decimal.TryParse(userIdClaim.Value, out var idUtilizador))
                    return Unauthorized(new { message = "Utilizador não autenticado." });

                // 2) Encontrar a tarefa
                var tarefa = await _context.Tarefas.FindAsync(id);
                if (tarefa == null)
                    return NotFound(new { message = "Tarefa não encontrada." });

                // 3) Verificar permissão de quem vai apagar
                var projetoTarefa = await _context.ProjetoTarefa
                    .FirstOrDefaultAsync(pt => pt.TarefaId == id);

                if (projetoTarefa != null)
                {
                    var projeto = await _context.Projetos
                        .FirstOrDefaultAsync(p => p.IdProjeto == projetoTarefa.IdProjeto);

                    var isCriador = projeto.IdUtilizador == idUtilizador;
                    var isResponsavel = tarefa.IdUtilizador == idUtilizador;
                    var isMembroEquipa = false;

                    if (projeto.EquipaId.HasValue)
                    {
                        isMembroEquipa = await _context.EquipaUtilizadores
                            .AnyAsync(eu => eu.EquipaId == projeto.EquipaId
                                         && eu.UtilizadorId == idUtilizador);
                    }

                    if (!isCriador && !isResponsavel && !isMembroEquipa)
                        return Unauthorized(new { message = "Sem permissão para eliminar esta tarefa." });
                }
                else
                {
                    // Se não estiver associado a nenhum ProjetoTarefa, negar.
                    return Unauthorized(new { message = "Tarefa não está ligada a nenhum projeto ou sem permissão." });
                }

                // 4) Remover vínculo em ProjetoTarefa
                _context.ProjetoTarefa.Remove(projetoTarefa);
                // 5) Remover a tarefa
                _context.Tarefas.Remove(tarefa);

                await _context.SaveChangesAsync();
                return Ok(new { message = "Tarefa eliminada com sucesso." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Erro interno ao eliminar tarefa.",
                    detalhe = ex.Message
                });
            }
        }

        // ----------------------------------------------
        // MembrosEquipe: obtém lista de membros de equipa
        // (para popular dropdown de Responsável)
        // ----------------------------------------------
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
                        nome = eu.Utilizador.Nome
                    })
                    .OrderBy(x => x.nome)
                    .ToListAsync();
            }
            else
            {
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
