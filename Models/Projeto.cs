using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication2.Entities;

public partial class Projeto
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal IdProjeto { get; set; }

    public string NomeProjeto { get; set; } = null!;

    public string NomeCliente { get; set; } = null!;

    public decimal? PrecoHora { get; set; }

    public decimal IdUtilizador { get; set; }
    
    public virtual Utilizador? IdUtilizadorNavigation { get; set; }

    public virtual ICollection<Membro> Membros { get; set; } = new List<Membro>();
    
}