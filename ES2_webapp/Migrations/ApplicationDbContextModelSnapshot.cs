﻿// <auto-generated />
using System;
using ES2_webapp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ES2_webapp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Convite", b =>
                {
                    b.Property<int>("IdMensagem")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdMensagem"));

                    b.Property<DateTime>("DataEnvio")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("FoiLido")
                        .HasColumnType("boolean");

                    b.Property<int>("IdEquipa")
                        .HasColumnType("integer");

                    b.Property<decimal>("IdUtilizadorDestinatario")
                        .HasColumnType("numeric");

                    b.Property<decimal>("IdUtilizadorRemetente")
                        .HasColumnType("numeric");

                    b.Property<string>("Mensagem")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool?>("Resposta")
                        .HasColumnType("boolean");

                    b.HasKey("IdMensagem");

                    b.HasIndex("IdEquipa");

                    b.HasIndex("IdUtilizadorDestinatario");

                    b.ToTable("Convites");
                });

            modelBuilder.Entity("Equipa", b =>
                {
                    b.Property<int>("IdEquipa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdEquipa"));

                    b.Property<decimal>("IdCriador")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("NHabitualHoras")
                        .HasColumnType("numeric");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdEquipa");

                    b.ToTable("Equipas");
                });

            modelBuilder.Entity("EquipaUtilizador", b =>
                {
                    b.Property<int>("EquipaId")
                        .HasColumnType("integer");

                    b.Property<decimal>("UtilizadorId")
                        .HasColumnType("numeric");

                    b.HasKey("EquipaId", "UtilizadorId");

                    b.HasIndex("UtilizadorId");

                    b.ToTable("EquipaUtilizadores");
                });

            modelBuilder.Entity("Projeto", b =>
                {
                    b.Property<int>("IdProjeto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdProjeto"));

                    b.Property<int?>("EquipaId")
                        .HasColumnType("integer");

                    b.Property<decimal>("IdUtilizador")
                        .HasColumnType("numeric");

                    b.Property<string>("NomeCliente")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NomeProjeto")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdProjeto");

                    b.HasIndex("EquipaId");

                    b.HasIndex("IdUtilizador");

                    b.ToTable("Projetos");
                });

            modelBuilder.Entity("ProjetoTarefa", b =>
                {
                    b.Property<int>("IdProjeto")
                        .HasColumnType("integer");

                    b.Property<int>("TarefaId")
                        .HasColumnType("integer");

                    b.HasKey("IdProjeto", "TarefaId");

                    b.HasIndex("TarefaId");

                    b.ToTable("ProjetoTarefa");
                });

            modelBuilder.Entity("Tarefa", b =>
                {
                    b.Property<int>("IdTarefa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdTarefa"));

                    b.Property<DateOnly?>("DtFim")
                        .HasColumnType("date");

                    b.Property<DateOnly?>("DtInicio")
                        .HasColumnType("date");

                    b.Property<TimeSpan>("HrFim")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("HrInicio")
                        .HasColumnType("interval");

                    b.Property<decimal?>("IdUtilizador")
                        .HasColumnType("numeric");

                    b.Property<string>("NomeTarefa")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("PrecoHora")
                        .HasColumnType("numeric");

                    b.HasKey("IdTarefa");

                    b.ToTable("Tarefas");
                });

            modelBuilder.Entity("WebApplication2.Entities.Utilizador", b =>
                {
                    b.Property<decimal>("IdUtilizador")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric");

                    b.Property<decimal>("CargoId")
                        .HasColumnType("numeric");

                    b.Property<decimal?>("NHabitualHoras")
                        .HasColumnType("numeric");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdUtilizador");

                    b.HasIndex("CargoId");

                    b.ToTable("Utilizadores");
                });

            modelBuilder.Entity("WebApplication2.Models.Cargo", b =>
                {
                    b.Property<decimal>("IdCargo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("UtilizadorIdUtilizador")
                        .HasColumnType("numeric");

                    b.HasKey("IdCargo");

                    b.HasIndex("UtilizadorIdUtilizador");

                    b.ToTable("Cargos");
                });

            modelBuilder.Entity("Convite", b =>
                {
                    b.HasOne("Equipa", "Equipa")
                        .WithMany("Convites")
                        .HasForeignKey("IdEquipa")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Entities.Utilizador", "Utilizador")
                        .WithMany()
                        .HasForeignKey("IdUtilizadorDestinatario")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipa");

                    b.Navigation("Utilizador");
                });

            modelBuilder.Entity("EquipaUtilizador", b =>
                {
                    b.HasOne("Equipa", "Equipa")
                        .WithMany("Membros")
                        .HasForeignKey("EquipaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("WebApplication2.Entities.Utilizador", "Utilizador")
                        .WithMany()
                        .HasForeignKey("UtilizadorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Equipa");

                    b.Navigation("Utilizador");
                });

            modelBuilder.Entity("Projeto", b =>
                {
                    b.HasOne("Equipa", "Equipa")
                        .WithMany("Projetos")
                        .HasForeignKey("EquipaId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.HasOne("WebApplication2.Entities.Utilizador", "Criador")
                        .WithMany("Projetos")
                        .HasForeignKey("IdUtilizador")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Criador");

                    b.Navigation("Equipa");
                });

            modelBuilder.Entity("ProjetoTarefa", b =>
                {
                    b.HasOne("Projeto", "Projeto")
                        .WithMany("ProjetosTarefas")
                        .HasForeignKey("IdProjeto")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Tarefa", "Tarefa")
                        .WithMany("ProjetosTarefas")
                        .HasForeignKey("TarefaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Projeto");

                    b.Navigation("Tarefa");
                });

            modelBuilder.Entity("WebApplication2.Entities.Utilizador", b =>
                {
                    b.HasOne("WebApplication2.Models.Cargo", "Cargo")
                        .WithMany()
                        .HasForeignKey("CargoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Cargo");
                });

            modelBuilder.Entity("WebApplication2.Models.Cargo", b =>
                {
                    b.HasOne("WebApplication2.Entities.Utilizador", null)
                        .WithMany("Cargos")
                        .HasForeignKey("UtilizadorIdUtilizador");
                });

            modelBuilder.Entity("Equipa", b =>
                {
                    b.Navigation("Convites");

                    b.Navigation("Membros");

                    b.Navigation("Projetos");
                });

            modelBuilder.Entity("Projeto", b =>
                {
                    b.Navigation("ProjetosTarefas");
                });

            modelBuilder.Entity("Tarefa", b =>
                {
                    b.Navigation("ProjetosTarefas");
                });

            modelBuilder.Entity("WebApplication2.Entities.Utilizador", b =>
                {
                    b.Navigation("Cargos");

                    b.Navigation("Projetos");
                });
#pragma warning restore 612, 618
        }
    }
}
