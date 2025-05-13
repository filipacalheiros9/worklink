namespace WebApplication2.DTO;

public class TarefasIndCreate
{
    public string NomeTarefa { get; set; }
    public DateOnly? DtInicio { get; set; }
    public TimeSpan HrInicio { get; set; }
    public DateOnly? DtFim { get; set; }
    public TimeSpan HrFim { get; set; }
    public decimal IdProjeto { get; set; }
}