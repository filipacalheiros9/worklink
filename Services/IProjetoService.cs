using WebApplication2.Entities;
using  WebApplication2.DTO;

namespace WebApplication2.Services;

public interface IProjetoService
{
    Task<Projeto?> GetProjetoByIdAsync(int id);
    Task<IEnumerable<Projeto>> GetAllProjetosAsync();
    Task AddProjetoAsync(Projeto projeto);
    Task UpdateProjetoAsync(Projeto projeto);
    Task DeleteProjetoAsync(int id);
    List<Projeto> ObterTodosProjetos();
}