namespace WebApplication2.DTO;

public class TarefaEditDto
{
    public string NomeTarefa { get; set; } = string.Empty;
    public DateOnly? DtFim { get; set; }
    public TimeSpan? HrFim { get; set; }
    public decimal PrecoHora { get; set; }
}
