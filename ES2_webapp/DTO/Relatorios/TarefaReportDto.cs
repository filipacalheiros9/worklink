namespace ES2_webapp.DTO.Relatorios
{
    public class TarefaReportDto
    {
        public string Nome { get; set; }
        public DateOnly? DataInicio { get; set; }
        public DateOnly? DataFim { get; set; }
        public TimeSpan HoraInicio { get; set; }
        public TimeSpan HoraFim { get; set; }
        public decimal? PrecoHora { get; set; }
        public string Projeto { get; set; }
    }
} 