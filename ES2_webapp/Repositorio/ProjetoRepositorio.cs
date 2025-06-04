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

    // ✅ Apenas os projetos do utilizador que NÃO estão associados a equipa
    public List<Projeto> ObterProjetosPessoais(decimal idUtilizador)
    {
        return _context.Projetos
            .Where(p => p.IdUtilizador == idUtilizador && p.EquipaId == null)
            .ToList();
    }

    // ✅ Projetos de equipas às quais o utilizador pertence
    public List<Projeto> ObterProjetosEquipa(decimal idUtilizador)
    {
        return _context.EquipaUtilizadores
            .Where(eu => eu.UtilizadorId == idUtilizador)
            .Join(_context.Projetos,
                eu => eu.EquipaId,
                p => p.EquipaId,
                (eu, p) => p)
            .ToList<Projeto>(); // 👈 resolve a ambiguidade do tipo
    }

}
