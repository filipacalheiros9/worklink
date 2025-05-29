using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Entities;
public class ProjetoTarefa
{
    [ForeignKey(nameof(Projeto))]
    public int IdProjeto { get; set; }
    public Projeto Projeto { get; set; } = null!;

    [ForeignKey(nameof(Tarefa))]
    public int TarefaId { get; set; }
    public Tarefa Tarefa { get; set; } = null!;
}