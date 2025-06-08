using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ES2_webapp.Data;
using ES2_webapp.DTO.Relatorios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                actionName:     "RelatorioMensal",
                controllerName: "RelatoriosView",
                routeValues:    new { mes, ano, idUtilizador }
            );
        }

        [HttpGet("RelatorioMensalPorUtilizador")]
        public async Task<IActionResult> RelatorioMensal(int mes, int ano, int idUtilizador)
        {
            if (mes < 1 || mes > 12)
                return BadRequest("O parâmetro 'mes' deve estar entre 1 e 12.");
            if (ano < 2000)
                return BadRequest("O parâmetro 'ano' deve ser um ano válido (>= 2000).");

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

            var tarefasIndividuais = todasTarefas
                .Where(t => t.ProjetosTarefas == null || t.ProjetosTarefas.Count == 0)
                .ToList();

            var tarefasProjeto = todasTarefas
                .Where(t => t.ProjetosTarefas != null && t.ProjetosTarefas.Count > 0)
                .ToList();

            var tarefas = tarefasIndividuais.Union(tarefasProjeto).ToList();

            var relatorio = new MonthlyReportDto
            {
                Mes                     = mes,
                Ano                     = ano,
                HorasDiariasPermitidas  = 8m,
                Dias                    = new List<DayReportDto>()
            };

            int diasNoMes = DateTime.DaysInMonth(ano, mes);
            for (int dia = 1; dia <= diasNoMes; dia++)
            {
                var dt = new DateOnly(ano, mes, dia);
                relatorio.Dias.Add(new DayReportDto
                {
                    Dia               = dt.ToDateTime(TimeOnly.MinValue),
                    TotalHoras        = 0m,
                    TotalCusto        = 0m,
                    ExcedeuHoras      = false,
                    TarefasDoDia      = new List<TaskReportItemDto>()
                });
            }

            foreach (var tarefa in tarefas)
            {
                var dataRef = tarefa.DtInicio ?? tarefa.DtFim ?? new DateOnly(ano, mes, 1);
                var hi      = tarefa.HrInicio is TimeSpan tsi ? TimeOnly.FromTimeSpan(tsi) : TimeOnly.MinValue;
                var hf      = tarefa.HrFim    is TimeSpan tsf ? TimeOnly.FromTimeSpan(tsf) : TimeOnly.MinValue;

                var dtInicio = dataRef.ToDateTime(hi);
                var dtFim    = dataRef.ToDateTime(hf);
                var durHoras = (dtFim - dtInicio).TotalHours;
                if (durHoras < 0) durHoras = 0;
                var horas    = (decimal)durHoras;
                var custo    = horas * (tarefa.PrecoHora ?? 0m);

                var nomeProj = tarefa.ProjetosTarefas
                                .Select(pt => pt.Projeto?.NomeProjeto)
                                .FirstOrDefault()
                             ?? "– sem projeto –";

                var item = new TaskReportItemDto
                {
                    Data        = dataRef.ToDateTime(TimeOnly.MinValue),
                    NomeProjeto = nomeProj,
                    NomeTarefa  = tarefa.NomeTarefa,
                    Horas       = horas,
                    Custo       = custo
                };

                var diaDto = relatorio.Dias
                    .FirstOrDefault(d => d.Dia.Date == dataRef.ToDateTime(TimeOnly.MinValue).Date);
                if (diaDto == null)
                {
                    diaDto = new DayReportDto
                    {
                        Dia = dataRef.ToDateTime(TimeOnly.MinValue),
                        TotalHoras = 0m,
                        TotalCusto = 0m,
                        ExcedeuHoras = false,
                        TarefasDoDia = new List<TaskReportItemDto>()
                    };
                    relatorio.Dias.Add(diaDto);
                }
                diaDto.TarefasDoDia.Add(item);
                diaDto.TotalHoras += horas;
                diaDto.TotalCusto += custo;
            }

            foreach (var dia in relatorio.Dias)
                dia.ExcedeuHoras = dia.TotalHoras > relatorio.HorasDiariasPermitidas;

            relatorio.HorasMesTotal = relatorio.Dias.Sum(d => d.TotalHoras);
            relatorio.CustoMesTotal = relatorio.Dias.Sum(d => d.TotalCusto);

            return View(relatorio);
        }
    }
}
