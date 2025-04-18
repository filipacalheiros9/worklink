using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities; 

public partial class Membro
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal IdMembro { get; set; }

    public string Nome { get; set; } = null!;

    public string Password { get; set; } = null!;

    public decimal NHabitualHoras { get; set; }

    public decimal IdUtilizador { get; set; }

    public decimal IdProjeto { get; set; }

    public virtual Projeto IdProjetoNavigation { get; set; } = null!;

    public virtual Utilizador IdUtilizadorNavigation { get; set; } = null!;
}

