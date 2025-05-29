using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities;

public class Convite
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdMensagem { get; set; }

    [Required]
    public string Mensagem { get; set; } = string.Empty;

    // null = pendente | true = aceite | false = recusado
    public bool? Resposta { get; set; }

    public bool FoiLido { get; set; } = false;

    public decimal IdUtilizadorRemetente { get; set; }

    public decimal IdUtilizadorDestinatario { get; set; }

    [ForeignKey("IdUtilizadorDestinatario")]
    public virtual Utilizador? Utilizador { get; set; }

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;
    
    public int IdEquipa { get; set; }

    [ForeignKey("IdEquipa")]
    public Equipa Equipa { get; set; } = null!;
}