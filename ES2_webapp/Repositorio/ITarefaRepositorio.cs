namespace WebApplication2.Data.Repositories;

public interface ITarefaRepository
{
    Task<Tarefa?> GetByIdAsync(int id);
    Task<IEnumerable<Tarefa>> GetAllAsync();
    Task AddAsync(Tarefa tarefa);
    Task UpdateAsync(Tarefa tarefa);
    Task DeleteAsync(int id);
}