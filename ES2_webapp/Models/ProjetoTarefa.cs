using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities;
public class ProjetoTarefa
{
    public decimal IdProjeto { get; set; }
    public Projeto Projeto { get; set; } = null!;

    public int  TarefaId { get; set; }
    public Tarefa Tarefa { get; set; } = null!;
}
