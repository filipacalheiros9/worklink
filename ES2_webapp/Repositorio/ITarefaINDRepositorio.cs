using WebApplication2.Entities;

namespace WebApplication2.Data.Repositories;

public interface ITarefaINDRepositorio
{
    List<Tarefa> ObterTarefasIndividuais();
    Tarefa ObterPorId(int id);
    void AtualizarTarefa(Tarefa tarefa);
}