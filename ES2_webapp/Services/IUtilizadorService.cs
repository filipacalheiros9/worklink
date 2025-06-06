using  WebApplication2.DTO;
using WebApplication2.Entities;

namespace WebApplication2.Services
{
    public interface IUtilizadorService
    {
        Task<Utilizador?> GetUtilizadorByIdAsync(decimal id);
        Task<Utilizador?> ValidateLoginAsync(string username, string password);
        Task RegisterUtilizadorAsync(Utilizador utilizador);
        Task AtualizarPerfil(decimal idUtilizador, UtilizadorUpdate model);

        Task<int> CountUtilizadoresAsync();
        
        Task<List<Utilizador>> GetAllUtilizadoresAsync();
        Task DeleteUtilizadorAsync(decimal id);

    }
}