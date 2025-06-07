using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ES2_webapp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ES2_webapp.DTO.Relatorios;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ES2_webapp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelatoriosController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RelatoriosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("RelatorioMensalRedirecionar")]
        public IActionResult RelatorioMensalRedirecionar(int mes, int ano, int idUtilizador)
        {
            return RedirectToAction(
                actionName:   "RelatorioMensal",
                controllerName: "RelatoriosView",
                routeValues:  new { mes, ano, idUtilizador }
            );
        }

        [HttpGet]
        public async Task<IActionResult> RelatorioMensal(int mes, int ano, int idUtilizador)
        {
            if (mes < 1 || mes > 12)
                return BadRequest("O parâmetro 'mes' deve estar entre 1 e 12.");
            if (ano < 2000)
                return BadRequest("O parâmetro 'ano' deve ser um ano válido (>= 2000).");

            // === ALTERAÇÃO AQUI ===
            // Antes: só t.IdUtilizador == idUtilizador
            // Agora incluímos também tarefas de projetos criados por idUtilizador
            var todasTarefas = await _context.Tarefas
                .AsNoTracking()
                .Include(t => t.ProjetosTarefas)
                    .ThenInclude(pt => pt.Projeto)
                .Where(t =>
                    (
                        t.IdUtilizador == idUtilizador
                        || t.ProjetosTarefas.Any(pt => pt.Projeto.IdUtilizador == idUtilizador)
                    )
                    &&
                    (
                        (t.DtInicio.HasValue && t.DtInicio.Value.Year == ano && t.DtInicio.Value.Month == mes)
                        ||
                        (t.DtFim.HasValue     && t.DtFim.Value.Year     == ano && t.DtFim.Value.Month     == mes)
                    )
                )
                .ToListAsync();
            // ========================

            // Separar tarefas individuais e de projeto
            var tarefasIndividuais = todasTarefas.Where(t => t.ProjetosTarefas == null || t.ProjetosTarefas.Count == 0).ToList();
            var tarefasProjeto = todasTarefas.Where(t => t.ProjetosTarefas != null && t.ProjetosTarefas.Count > 0).ToList();

            // Juntar tudo para o relatório final
            var tarefas = tarefasIndividuais.Union(tarefasProjeto).ToList();

            var relatorio = new MonthlyReportDto
            {
                Mes = mes,
                Ano = ano,
                HorasDiariasPermitidas = 8m,
                Dias = new List<DayReportDto>()
            };

            int diasNoMes = DateTime.DaysInMonth(ano, mes);
            for (int dia = 1; dia <= diasNoMes; dia++)
            {
                DateOnly diaOnly = new DateOnly(ano, mes, dia);
                DateTime diaDateTime = diaOnly.ToDateTime(TimeOnly.MinValue);

                relatorio.Dias.Add(new DayReportDto
                {
                    Dia = diaDateTime,
                    TotalHoras = 0m,
                    TotalCusto = 0m,
                    ExcedeuHoras = false,
                    TarefasDoDia = new List<TaskReportItemDto>()
                });
            }

            foreach (var tarefa in tarefas)
            {
                DateOnly dataReferencia = tarefa.DtInicio ?? tarefa.DtFim ?? new DateOnly(ano, mes, 1);
                TimeOnly horaFimOnly = TimeOnly.MinValue;
                TimeOnly horaInicioOnly = TimeOnly.MinValue;

                if (tarefa.HrFim is TimeSpan tsFim)
                    horaFimOnly = TimeOnly.FromTimeSpan(tsFim);
                if (tarefa.HrInicio is TimeSpan tsInicio)
                    horaInicioOnly = TimeOnly.FromTimeSpan(tsInicio);

                DateTime fimCompleto = dataReferencia.ToDateTime(horaFimOnly);
                DateTime inicioCompleto = dataReferencia.ToDateTime(horaInicioOnly);

                double duracaoHorasDouble = (fimCompleto - inicioCompleto).TotalHours;
                if (duracaoHorasDouble < 0)
                    duracaoHorasDouble = 0;
                decimal horas = (decimal)duracaoHorasDouble;

                decimal precoHora = tarefa.PrecoHora ?? 0m;
                decimal custo = horas * precoHora;

                string nomeProjeto = tarefa.ProjetosTarefas
                    .Select(pt => pt.Projeto != null ? pt.Projeto.NomeProjeto : null)
                    .FirstOrDefault()
                    ?? "– sem projeto –";

                var itemDto = new TaskReportItemDto
                {
                    Data = dataReferencia.ToDateTime(TimeOnly.MinValue),
                    NomeProjeto = nomeProjeto,
                    NomeTarefa = tarefa.NomeTarefa,
                    Horas = horas,
                    Custo = custo
                };

                var diaDto = relatorio.Dias.FirstOrDefault(d => d.Dia.Date == dataReferencia.ToDateTime(TimeOnly.MinValue).Date);
                if (diaDto != null)
                {
                    diaDto.TarefasDoDia.Add(itemDto);
                    diaDto.TotalHoras += horas;
                    diaDto.TotalCusto += custo;
                }
            }

            foreach (var dia in relatorio.Dias)
            {
                dia.ExcedeuHoras = dia.TotalHoras > relatorio.HorasDiariasPermitidas;
            }

            relatorio.HorasMesTotal = relatorio.Dias.Sum(d => d.TotalHoras);
            relatorio.CustoMesTotal = relatorio.Dias.Sum(d => d.TotalCusto);

            return View(relatorio);
        }

        public async Task<IActionResult> GetMonthlyReport(int mes, int ano)
        {
            var idUtilizador = HttpContext.Session.GetInt32("UserId");
            if (!idUtilizador.HasValue)
            {
                return RedirectToAction("Login", "Home");
            }
            
            var tarefas = await _context.Tarefas
                .AsNoTracking()
                .Include(t => t.ProjetosTarefas)
                    .ThenInclude(pt => pt.Projeto)
                .Where(t =>
                    (
                        t.IdUtilizador == idUtilizador.Value
                        || t.ProjetosTarefas.Any(pt => pt.Projeto.IdUtilizador == idUtilizador.Value)
                    )
                    &&
                    (
                        (t.DtInicio.HasValue && t.DtInicio.Value.Year == ano && t.DtInicio.Value.Month == mes)
                        ||
                        (t.DtFim.HasValue     && t.DtFim.Value.Year     == ano && t.DtFim.Value.Month     == mes)
                    )
                )
                .ToListAsync();

            var relatorio = new MonthlyReportDto
            {
                Mes = mes,
                Ano = ano,
                Tarefas = tarefas.Select(t => new TarefaReportDto
                {
                    Nome = t.NomeTarefa,
                    DataInicio = t.DtInicio,
                    DataFim = t.DtFim,
                    HoraInicio = t.HrInicio,
                    HoraFim = t.HrFim,
                    PrecoHora = t.PrecoHora,
                    Projeto = t.ProjetosTarefas.FirstOrDefault()?.Projeto?.NomeProjeto ?? "Tarefa Individual"
                }).ToList()
            };

            return View(relatorio);
        }

        [HttpGet("RelatorioMensalProjetoCliente")]
        public async Task<IActionResult> RelatorioMensalProjetoCliente(int mes, int ano)
        {
            var tarefas = await _context.Tarefas
                .Include(t => t.ProjetosTarefas)
                    .ThenInclude(pt => pt.Projeto)
                        .ThenInclude(p => p.Cliente)
                .Include(t => t.Utilizador)
                .Where(t =>
                    (t.DtInicio.HasValue && t.DtInicio.Value.Year == ano && t.DtInicio.Value.Month == mes) ||
                    (t.DtFim.HasValue && t.DtFim.Value.Year == ano && t.DtFim.Value.Month == mes)
                )
                .ToListAsync();

            var resultado = tarefas
                .SelectMany(t => t.ProjetosTarefas.Select(pt => new {
                    Cliente = pt.Projeto.Cliente,
                    Projeto = pt.Projeto,
                    Tarefa = t,
                    Utilizador = t.Utilizador
                }))
                .GroupBy(x => x.Cliente.NomeCliente)
                .Select(gCliente => new ProjetoClienteReportDto {
                    NomeCliente = gCliente.Key,
                    Projetos = gCliente.GroupBy(x => x.Projeto.NomeProjeto)
                        .Select(gProjeto => new ProjetoReportDto {
                            NomeProjeto = gProjeto.Key,
                            Dias = gProjeto.GroupBy(x => x.Tarefa.DtInicio)
                                .Select(gDia => new DayProjetoReportDto {
                                    Dia = gDia.Key.Value.ToDateTime(TimeOnly.MinValue),
                                    Utilizadores = gDia.GroupBy(x => x.Utilizador.Nome)
                                        .Select(gUser => new UtilizadorTarefaReportDto {
                                            NomeUtilizador = gUser.Key,
                                            Tarefas = gUser.Select(x => new TarefaProjetoReportDto {
                                                NomeTarefa = x.Tarefa.NomeTarefa,
                                                Horas = (decimal)((x.Tarefa.DtFim.Value.ToDateTime(TimeOnly.FromTimeSpan(x.Tarefa.HrFim)) - x.Tarefa.DtInicio.Value.ToDateTime(TimeOnly.FromTimeSpan(x.Tarefa.HrInicio))).TotalHours),
                                                Custo = ((decimal)((x.Tarefa.DtFim.Value.ToDateTime(TimeOnly.FromTimeSpan(x.Tarefa.HrFim)) - x.Tarefa.DtInicio.Value.ToDateTime(TimeOnly.FromTimeSpan(x.Tarefa.HrInicio))).TotalHours)) * (x.Tarefa.PrecoHora ?? 0)
                                            }).ToList()
                                        }).ToList()
                                }).ToList()
                        }).ToList()
                }).ToList();

            return Ok(resultado);
        }
    }
}
