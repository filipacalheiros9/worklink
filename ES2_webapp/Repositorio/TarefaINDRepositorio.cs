using ES2_webapp.Data;

namespace WebApplication2.Data.Repositories
{
    public class TarefaINDRepositorio : ITarefaINDRepositorio
    {
        private readonly ApplicationDbContext _context;

        public TarefaINDRepositorio(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Tarefa> ObterTarefasIndividuais()
        {
            // Tarefas que não têm NENHUM projeto associado
            return _context.Tarefas
                .Where(t => !t.ProjetosTarefas.Any())
                .ToList();
        }

        public Tarefa ObterPorId(int id)
        {
            return _context.Tarefas.FirstOrDefault(t => t.IdTarefa == id);
        }

        public void AtualizarTarefa(Tarefa tarefa)
        {
            _context.Tarefas.Update(tarefa);
            _context.SaveChanges();
        }
    }
}