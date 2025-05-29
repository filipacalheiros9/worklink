using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities;

public class Projeto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdProjeto { get; set; }

    public string NomeProjeto { get; set; } = string.Empty;
    public string NomeCliente { get; set; } = string.Empty;
    
    public decimal IdUtilizador { get; set; }

    [ForeignKey("IdUtilizador")]
    public Utilizador? Criador { get; set; }
    
    public int? EquipaId { get; set; }
    public Equipa? Equipa { get; set; }
    public ICollection<ProjetoTarefa> ProjetosTarefas { get; set; } = new List<ProjetoTarefa>();
}