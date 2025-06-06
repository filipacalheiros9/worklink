using WebApplication2.Entities;

namespace WebApplication2.Services;

public interface IProjetoService
{
    Task<Projeto?> GetProjetoByIdAsync(int id);
    Task<IEnumerable<Projeto>> GetAllProjetosAsync();
    Task AddProjetoAsync(Projeto projeto);
    Task UpdateProjetoAsync(Projeto projeto);
    Task DeleteProjetoAsync(int id);

    // 🟢 Métodos novos que precisas de declarar
    List<Projeto> ObterProjetosPessoais(decimal idUtilizador);
    List<Projeto> ObterProjetosEquipa(decimal idUtilizador);
    List<Projeto> ObterProjetosVisiveis(decimal idUtilizador);
    Task<List<Projeto>> GetAllWithCriadorAsync();

}