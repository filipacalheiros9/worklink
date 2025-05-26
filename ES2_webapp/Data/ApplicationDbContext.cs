using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace ES2_webapp.Data;

public class ApplicationDbContext: DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<Utilizador> Utilizadores { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Convites> Convites { get; set; }
    public DbSet<Equipas> Equipas { get; set; }
    public DbSet<ProjetoTarefa> ProjetoTarefa { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Projeto>(entity =>
        {
            entity.HasKey(e => e.IdProjeto);

            entity.Property(e => e.NomeProjeto).IsRequired();
            entity.Property(e => e.NomeCliente).IsRequired();
            
            
            
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ProjetoTarefa>()
                .HasKey(pt => new { pt.IdProjeto, pt.TarefaId });

            modelBuilder.Entity<ProjetoTarefa>()
                .HasOne(pt => pt.Projeto)
                .WithMany(p => p.ProjetosTarefas)
                .HasForeignKey(pt => pt.IdProjeto);

            modelBuilder.Entity<ProjetoTarefa>()
                .HasOne(pt => pt.Tarefa)
                .WithMany(t => t.ProjetosTarefas)
                .HasForeignKey(pt => pt.TarefaId);
            
            modelBuilder.Entity<Projeto>()
                .HasOne(p => p.IdUtilizadorNavigation)
                .WithMany(u => u.Projetos)
                .HasForeignKey(p => p.IdUtilizador)
                .OnDelete(DeleteBehavior.Restrict);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Convites>();
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
    
}