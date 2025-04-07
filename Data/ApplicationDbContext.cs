using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using WebApplication2.Entities;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Membro> Membros { get; set; }
        public DbSet<Projeto> Projetos { get; set; }
        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Utilizador> Utilizadores { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Membro>().HasKey(m => m.IdMembro);
            modelBuilder.Entity<Projeto>().HasKey(p => p.IdProjeto);
            modelBuilder.Entity<Tarefa>().HasKey(t => t.IdTarefa);
            modelBuilder.Entity<Utilizador>()
                .HasKey(u => u.IdUtilizador);

            modelBuilder.Entity<Utilizador>()
                .Property(u => u.IdUtilizador)
                .ValueGeneratedOnAdd();
            // Additional model configurations can be added here
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }
    }
}