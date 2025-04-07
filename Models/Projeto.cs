using System;
using System.Collections.Generic;

namespace WebApplication2.Entities;

public partial class Projeto
{
    public decimal IdProjeto { get; set; }

    public string NomeProjeto { get; set; } = null!;

    public string NomeCliente { get; set; } = null!;

    public decimal? PrecoHora { get; set; }

    public decimal IdUtilizador { get; set; }

    public virtual Utilizador IdUtilizadorNavigation { get; set; } = null!;

    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();
}
