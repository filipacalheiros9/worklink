using ES2_webapp.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Entities;

namespace WebApplication2.Data.Repositories;

public class ProjetoRepository : IProjetoRepositorio
{
    private readonly ApplicationDbContext _context;

    public ProjetoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Projeto?> GetByIdAsync(int id)
    {
        return await _context.Projetos.FindAsync(id);
    }

    public async Task<IEnumerable<Projeto>> GetAllAsync()
    {
        return await _context.Projetos.ToListAsync();
    }

    public async Task AddAsync(Projeto projeto)
    {
        await _context.Projetos.AddAsync(projeto);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Projeto projeto)
    {
        _context.Projetos.Update(projeto);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var projeto = await GetByIdAsync(id);
        if (projeto != null)
        {
            _context.Projetos.Remove(projeto);
            await _context.SaveChangesAsync();
        }
    }
    
    public List<Projeto> ObterTodosProjetos()
    {
        return _context.Projetos.ToList();
    }
}