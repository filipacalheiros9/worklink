using WebApplication2.Data;
using WebApplication2.Data.Repositories;
using WebApplication2.DTO;
using WebApplication2.Entities;

namespace WebApplication2.Services
{
    public class UtilizadorService : IUtilizadorService
    {
        private readonly IUtilizadorRepository _utilizadorRepository;
        

        public UtilizadorService(IUtilizadorRepository utilizadorRepository)
        {
            _utilizadorRepository = utilizadorRepository;
        }

        public async Task<Utilizador?> GetUtilizadorByIdAsync(decimal id)
        {
            return await _utilizadorRepository.GetByIdAsync(id);
        }

        public async Task<Utilizador?> ValidateLoginAsync(string username, string password)
        {
            return await _utilizadorRepository.GetByCredentialsAsync(username, password);
        }

        public async Task RegisterUtilizadorAsync(Utilizador utilizador)
        {
            await _utilizadorRepository.AddAsync(utilizador);
        }
        
        public async Task AtualizarPerfil(decimal idUtilizador, UtilizadorUpdate model)
        {
            var utilizador = await _utilizadorRepository.GetByIdAsync(idUtilizador);
            if (utilizador == null)
                throw new Exception("Utilizador não encontrado.");

            utilizador.Nome = model.Nome;
            utilizador.Username = model.Username;
            utilizador.Password = model.Password;

            await _utilizadorRepository.AtualizarUtilizador(utilizador);
        }
        
        public async Task<int> CountUtilizadoresAsync()
        {
            return await _utilizadorRepository.CountAsync();
        }
        
        public async Task<List<Utilizador>> GetAllUtilizadoresAsync()
        {
            return await _utilizadorRepository.GetAllAsync();
        }

        public async Task DeleteUtilizadorAsync(decimal id)
        {
            await _utilizadorRepository.DeleteAsync(id);
        }


        
    }
}