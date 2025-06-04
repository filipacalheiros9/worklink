namespace WebApplication2.DTO;

public class TarefaIndividualDto
{
    public string NomeTarefa { get; set; }
    public DateTime DtInicio { get; set; }
    public string HrInicio { get; set; }
    public decimal PrecoHora { get; set; }
    public int? IdUtilizadorResponsavel { get; set; }
}