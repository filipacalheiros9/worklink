using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities; 

public class Tarefa
{
    public int IdTarefa { get; set; }
    public string NomeTarefa { get; set; }
    public DateOnly? DtInicio { get; set; }
    
    public string? HrInicio { get; set; }
    
    public DateOnly? DtFim { get; set; }
    
    public string? HrFim { get; set; }
    
    public decimal IdProjeto { get; set; }
    public Projeto? Projeto { get; set; }  
}


