using ES2_webapp.Data;
using WebApplication2.Data.Repositories;
using WebApplication2.Models; 
using WebApplication2.Data;

namespace WebApplication2.Services
{
    public class TarefaIndService : ITarefaIndService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITarefaINDRepositorio _tarefaINDRepositorio;

        public TarefaIndService(ApplicationDbContext context, ITarefaINDRepositorio tarefaINDRepositorio)
        {
            _context = context;
            _tarefaINDRepositorio = tarefaINDRepositorio;
        }

        public List<Tarefa> ObterTarefasIndividuais()
        {
            return _context.Tarefas
                .Where(t => !t.ProjetosTarefas.Any())
                .ToList();
        }

        public void AtribuirProjeto(int idTarefa, int idProjeto)
        {
            var tarefa = _tarefaINDRepositorio.ObterPorId(idTarefa);
            var projeto = _context.Projetos.Find(idProjeto);

            if (tarefa != null && projeto != null)
            {
                var novaRelacao = new ProjetoTarefa
                {
                    TarefaId = tarefa.IdTarefa,
                    IdProjeto = projeto.IdProjeto
                };

                _context.ProjetoTarefa.Add(novaRelacao);
                _context.SaveChanges();
            }
        }
    }
}