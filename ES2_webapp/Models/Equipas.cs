using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Equipa
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdEquipa { get; set; }

    public string Nome { get; set; } = string.Empty;

    public decimal? NHabitualHoras { get; set; }

    public decimal IdCriador { get; set; }

    public ICollection<EquipaUtilizador> EquipaUtilizadores { get; set; }
    public ICollection<Convite> Convites { get; set; } = new List<Convite>();
    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}



