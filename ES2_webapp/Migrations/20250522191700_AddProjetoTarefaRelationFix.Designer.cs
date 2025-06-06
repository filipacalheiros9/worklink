﻿// <auto-generated />
using System;
using ES2_webapp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ES2_webapp.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250522191700_AddProjetoTarefaRelationFix")]
    partial class AddProjetoTarefaRelationFix
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ProjetoTarefa", b =>
                {
                    b.Property<decimal>("IdProjeto")
                        .HasColumnType("numeric");

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

                    b.Property<string>("NomeTarefa")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal?>("PrecoHora")
                        .HasColumnType("numeric");

                    b.HasKey("IdTarefa");

                    b.ToTable("Tarefas");
                });

            modelBuilder.Entity("WebApplication2.Entities.Projeto", b =>
                {
                    b.Property<decimal>("IdProjeto")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric");

                    b.Property<decimal>("IdUtilizador")
                        .HasColumnType("numeric");

                    b.Property<string>("NomeCliente")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NomeProjeto")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdProjeto");

                    b.HasIndex("IdUtilizador");

                    b.ToTable("Projetos");
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

            modelBuilder.Entity("WebApplication2.Models.Equipas", b =>
                {
                    b.Property<decimal>("idEquipa")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("numeric");

                    b.Property<decimal?>("NHabitualHoras")
                        .HasColumnType("numeric");

                    b.Property<string>("nome")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("idEquipa");

                    b.ToTable("Equipas");
                });

            modelBuilder.Entity("ProjetoTarefa", b =>
                {
                    b.HasOne("WebApplication2.Entities.Projeto", "Projeto")
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

            modelBuilder.Entity("WebApplication2.Entities.Projeto", b =>
                {
                    b.HasOne("WebApplication2.Entities.Utilizador", "IdUtilizadorNavigation")
                        .WithMany("Projetos")
                        .HasForeignKey("IdUtilizador")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("IdUtilizadorNavigation");
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

            modelBuilder.Entity("Tarefa", b =>
                {
                    b.Navigation("ProjetosTarefas");
                });

            modelBuilder.Entity("WebApplication2.Entities.Projeto", b =>
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
