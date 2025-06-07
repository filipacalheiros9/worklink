using System;
using System.Collections.Generic;

namespace ES2_webapp.DTO.Relatorios
{
    public class ProjetoClienteReportDto
    {
        public string NomeCliente { get; set; }
        public List<ProjetoReportDto> Projetos { get; set; }
    }

    public class ProjetoReportDto
    {
        public string NomeProjeto { get; set; }
        public List<DayProjetoReportDto> Dias { get; set; }
    }

    public class DayProjetoReportDto
    {
        public DateTime Dia { get; set; }
        public List<UtilizadorTarefaReportDto> Utilizadores { get; set; }
    }

    public class UtilizadorTarefaReportDto
    {
        public string NomeUtilizador { get; set; }
        public List<TarefaProjetoReportDto> Tarefas { get; set; }
    }

    public class TarefaProjetoReportDto
    {
        public string NomeTarefa { get; set; }
        public decimal Horas { get; set; }
        public decimal Custo { get; set; }
    }
} 