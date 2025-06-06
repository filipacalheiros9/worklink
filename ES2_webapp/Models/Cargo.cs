using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models;

public class Cargo
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public decimal CargoId { get; set; }  // ✔ Nome deve coincidir com FK em Utilizador

    public string Nome { get; set; } = null!;
}