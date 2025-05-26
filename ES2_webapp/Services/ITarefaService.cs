namespace WebApplication2.Services;

public interface ITarefaService
{
    Task<Tarefa?> GetTarefaByIdAsync(int id);
    Task<IEnumerable<Tarefa>> GetAllTarefasAsync();
    Task AddTarefaAsync(Tarefa tarefa);
    Task UpdateTarefaAsync(Tarefa tarefa);
    Task DeleteTarefaAsync(int id);
}