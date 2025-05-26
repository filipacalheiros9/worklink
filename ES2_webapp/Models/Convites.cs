using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities;

namespace WebApplication2.Models;

public class Convites
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    
    public decimal IdMensagem { get; set;}
    public string Mensagem { get; set; } = null!;
    public Boolean Resposta { get; set; }
    public decimal IdUtilizadorRemetente{ get; set;}
    public decimal IdUtilizador { get; set; }

    [ForeignKey("IdUtilizador")]
    public virtual Utilizador? Utilizador { get; set; }
}