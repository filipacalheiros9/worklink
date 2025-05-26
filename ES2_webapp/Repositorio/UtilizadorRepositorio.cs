using ES2_webapp.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;

namespace WebApplication2.Data.Repositories
{
    public class UtilizadorRepository : IUtilizadorRepository
    {
        private readonly ApplicationDbContext _context;

        public UtilizadorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Utilizador?> GetByIdAsync(decimal id)
        {
            return await _context.Utilizadores.FirstOrDefaultAsync(u => u.IdUtilizador == id);
        }

        public async Task<Utilizador?> GetByCredentialsAsync(string username, string password)
        {
            return await _context.Utilizadores.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task AddAsync(Utilizador utilizador)
        {
            _context.Utilizadores.Add(utilizador);
            await _context.SaveChangesAsync();
        }
        
        public async Task AtualizarUtilizador(Utilizador utilizador)
        {
            _context.Utilizadores.Update(utilizador);
            await _context.SaveChangesAsync();
        }
        public async Task<int> CountAsync()
        {
            return await _context.Utilizadores.CountAsync();
        }
    }
}