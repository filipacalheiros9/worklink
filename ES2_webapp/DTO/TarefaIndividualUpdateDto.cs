namespace WebApplication2.DTO;

public class TarefaIndividualUpdateDto
{
    public string NomeTarefa { get; set; }
    public DateTime? DtFim { get; set; }
    public string HrFim { get; set; }
    public decimal PrecoHora { get; set; }

    // NOVO: quando editar, poderemos também mudar o responsável
    public int? IdUtilizadorResponsavel { get; set; }
}