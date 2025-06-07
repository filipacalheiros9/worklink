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

            var tarefas = await _context.Tarefas
                .AsNoTracking()
                .Where(t =>
                    t.IdUtilizador == idUtilizador &&
                    (
                        (t.DtInicio.HasValue && 
                         t.DtInicio.Value.Year == ano && 
                         t.DtInicio.Value.Month == mes) ||
                        (t.DtFim.HasValue && 
                         t.DtFim.Value.Year == ano && 
                         t.DtFim.Value.Month == mes)
                    )
                )
                .Include(t => t.ProjetosTarefas)
                    .ThenInclude(pt => pt.Projeto)
                .ToListAsync();

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
    }
}
