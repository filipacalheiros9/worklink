using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WebApplication2.Entities;

public class Tarefa
{
    [Key]
    public int IdTarefa { get; set; }

    public string NomeTarefa { get; set; } = string.Empty;
    public DateOnly? DtInicio { get; set; }
    public TimeSpan HrInicio { get; set; }
    public DateOnly? DtFim { get; set; }
    public TimeSpan HrFim { get; set; }
    
    public decimal? PrecoHora { get; set; }
    
    public decimal? IdUtilizador { get; set; }
    [ForeignKey("IdUtilizador")]
    public Utilizador? Utilizador { get; set; }
    
    public ICollection<ProjetoTarefa> ProjetosTarefas { get; set; } = new List<ProjetoTarefa>();
}
