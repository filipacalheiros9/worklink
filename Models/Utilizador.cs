using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Entities;

public partial class Utilizador
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal IdUtilizador { get; set; }

    public string Nome { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }
    
    public string cargo { get; set; } = "Utilizador";

    public decimal? NHabitualHoras { get; set; }

    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();

    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}
