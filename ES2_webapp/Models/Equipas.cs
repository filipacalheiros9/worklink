using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;

public class Equipas
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public decimal idEquipa { get; set; }
    public string nome { get; set; } = null!;
    public decimal? NHabitualHoras { get; set; }
}