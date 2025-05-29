using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApplication2.Entities;
using WebApplication2.Models;

namespace ES2_webapp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Projeto> Projetos { get; set; }
    public DbSet<Tarefa> Tarefas { get; set; }
    public DbSet<Utilizador> Utilizadores { get; set; }
    public DbSet<Cargo> Cargos { get; set; }
    public DbSet<Convite> Convites { get; set; }
    public DbSet<Equipa> Equipas { get; set; }
    public DbSet<EquipaUtilizador> EquipaUtilizadores { get; set; }
    public DbSet<ProjetoTarefa> ProjetoTarefa { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Projeto → Utilizador
        modelBuilder.Entity<Projeto>()
            .HasOne(p => p.Criador)
            .WithMany(u => u.Projetos)
            .HasForeignKey(p => p.IdUtilizador)
            .OnDelete(DeleteBehavior.Restrict);

        // Projeto → Equipa (opcional)
        modelBuilder.Entity<Projeto>()
            .HasOne(p => p.Equipa)
            .WithMany(e => e.Projetos)
            .HasForeignKey(p => p.EquipaId)
            .OnDelete(DeleteBehavior.SetNull);

        // ProjetoTarefa chave composta
        modelBuilder.Entity<ProjetoTarefa>()
            .HasKey(pt => new { pt.IdProjeto, pt.TarefaId });

        modelBuilder.Entity<ProjetoTarefa>()
            .HasOne(pt => pt.Projeto)
            .WithMany(p => p.ProjetosTarefas)
            .HasForeignKey(pt => pt.IdProjeto)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ProjetoTarefa>()
            .HasOne(pt => pt.Tarefa)
            .WithMany(t => t.ProjetosTarefas)
            .HasForeignKey(pt => pt.TarefaId)
            .OnDelete(DeleteBehavior.Cascade);

        // EquipaUtilizador chave composta
        modelBuilder.Entity<EquipaUtilizador>()
            .HasKey(eu => new { eu.EquipaId, eu.UtilizadorId });

        modelBuilder.Entity<EquipaUtilizador>()
            .HasOne(eu => eu.Equipa)
            .WithMany(e => e.EquipaUtilizadores )
            .HasForeignKey(eu => eu.EquipaId);

        modelBuilder.Entity<EquipaUtilizador>()
            .HasOne(eu => eu.Utilizador)
            .WithMany()
            .HasForeignKey(eu => eu.UtilizadorId);

        // ✅ Convite → Equipa
        modelBuilder.Entity<Convite>()
            .HasOne(c => c.Equipa)
            .WithMany(e => e.Convites)
            .HasForeignKey(c => c.IdEquipa)
            .OnDelete(DeleteBehavior.Cascade);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.ConfigureWarnings(warnings =>
            warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
    }
}
