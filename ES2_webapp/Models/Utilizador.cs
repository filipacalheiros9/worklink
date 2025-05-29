using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;

namespace WebApplication2.Entities;

public partial class Utilizador
{

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal IdUtilizador { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Cargo Cargo { get; set; } = null!;
    public decimal CargoId { get; set; }
    public decimal? NHabitualHoras { get; set; }

    public virtual ICollection<Cargo> Cargos { get; set; } = new List<Cargo>();
    public ICollection<Projeto> Projetos { get; set; } = new List<Projeto>();
}