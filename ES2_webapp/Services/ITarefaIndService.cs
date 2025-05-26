using WebApplication2.Entities;
using  WebApplication2.DTO;

namespace WebApplication2.Services;

public interface ITarefaIndService
{
    List<Tarefa> ObterTarefasIndividuais();
    void AtribuirProjeto(int idTarefa, int idProjeto);

}