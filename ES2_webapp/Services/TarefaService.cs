using WebApplication2.Data.Repositories;

namespace WebApplication2.Services;

public class TarefaService : ITarefaService
{
    private readonly ITarefaRepository _tarefaRepository;

    public TarefaService(ITarefaRepository tarefaRepository)
    {
        _tarefaRepository = tarefaRepository;
    }

    public async Task<Tarefa?> GetTarefaByIdAsync(int id)
    {
        return await _tarefaRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Tarefa>> GetAllTarefasAsync()
    {
        return await _tarefaRepository.GetAllAsync();
    }

    public async Task AddTarefaAsync(Tarefa tarefa)
    {
        await _tarefaRepository.AddAsync(tarefa);
    }

    public async Task UpdateTarefaAsync(Tarefa tarefa)
    {
        await _tarefaRepository.UpdateAsync(tarefa);
    }

    public async Task DeleteTarefaAsync(int id)
    {
        await _tarefaRepository.DeleteAsync(id);
    }
}