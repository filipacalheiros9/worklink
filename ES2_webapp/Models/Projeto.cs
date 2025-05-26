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

    public string NomeProjeto { get; set; } = string.Empty; 

    public string NomeCliente { get; set; } = string.Empty; 

    public decimal IdUtilizador { get; set; }
    
    public virtual Utilizador? IdUtilizadorNavigation { get; set; }
    public ICollection<ProjetoTarefa> ProjetosTarefas { get; set; } = new List<ProjetoTarefa>();
    
}