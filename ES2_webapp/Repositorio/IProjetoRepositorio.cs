using WebApplication2.Entities;

namespace WebApplication2.Data.Repositories;

    public interface IProjetoRepositorio
    {
        Task<Projeto?> GetByIdAsync(int id);
        Task<IEnumerable<Projeto>> GetAllAsync();
        Task AddAsync(Projeto projeto);
        Task UpdateAsync(Projeto projeto);
        Task DeleteAsync(int id);
        List<Projeto> ObterProjetosPessoais(decimal idUtilizador);
        List<Projeto> ObterProjetosEquipa(decimal idUtilizador);
        Task<List<Projeto>> GetAllWithCriadorAsync();

    }
