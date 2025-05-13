using WebApplication2.Data.Repositories;
using WebApplication2.Entities;

namespace WebApplication2.Services;

public class ProjetoService : IProjetoService
{
    private readonly IProjetoRepositorio _projetoRepository;

    public ProjetoService(IProjetoRepositorio projetoRepository)
    {
        _projetoRepository = projetoRepository;
    }

    public async Task<Projeto?> GetProjetoByIdAsync(int id)
    {
        return await _projetoRepository.GetByIdAsync(id);
    }

    public async Task<IEnumerable<Projeto>> GetAllProjetosAsync()
    {
        return await _projetoRepository.GetAllAsync();
    }

    public async Task AddProjetoAsync(Projeto projeto)
    {
        await _projetoRepository.AddAsync(projeto);
    }

    public async Task UpdateProjetoAsync(Projeto projeto)
    {
        await _projetoRepository.UpdateAsync(projeto);
    }

    public async Task DeleteProjetoAsync(int id)
    {
        await _projetoRepository.DeleteAsync(id);
    }
    
    public List<Projeto> ObterTodosProjetos()
    {
        return _projetoRepository.ObterTodosProjetos();
    }
}