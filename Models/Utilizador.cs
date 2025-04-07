using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Utilizador
{
    public decimal IdUtilizador { get; set; }

    public string Nome { get; set; }

    public string Username { get; set; }

    public string Password { get; set; }

    public decimal? NHabitualHoras { get; set; }

    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();

    public virtual ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}
