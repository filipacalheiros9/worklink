using WebApplication2.Data.Repositories;
using WebApplication2.Models; 

namespace WebApplication2.Services
{
    public class TarefaIndService : ITarefaIndService
    {
        private readonly ITarefaINDRepositorio _tarefaINDRepositorio;

        // Construtor para injetar o repositório
        public TarefaIndService(ITarefaINDRepositorio tarefaINDRepositorio)
        {
            _tarefaINDRepositorio = tarefaINDRepositorio;
        }

        public List<Tarefa> ObterTarefasIndividuais()
        {
            return _tarefaINDRepositorio.ObterTarefasIndividuais();
        }

        public void AtribuirProjeto(int idTarefa, int idProjeto)
        {
            var tarefa = _tarefaINDRepositorio.ObterPorId(idTarefa);
            if (tarefa != null)
            {
                tarefa.IdProjeto = idProjeto;
                _tarefaINDRepositorio.AtualizarTarefa(tarefa);
            }
        }
    }
}