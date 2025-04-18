using WebApplication2.Entities;

namespace WebApplication2.Data.Repositories
{
    public interface IUtilizadorRepository
    {
        Task<Utilizador?> GetByIdAsync(decimal id);
        Task<Utilizador?> GetByCredentialsAsync(string username, string password);
        Task AddAsync(Utilizador utilizador);
        Task AtualizarUtilizador(Utilizador utilizador);
        Task<int> CountAsync();
    }
}