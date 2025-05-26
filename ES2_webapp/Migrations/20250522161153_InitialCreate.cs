using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ES2_webapp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cargos",
                columns: table => new
                {
                    IdCargo = table.Column<decimal>(type: "numeric", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cargos", x => x.IdCargo);
                });

            migrationBuilder.CreateTable(
                name: "Equipas",
                columns: table => new
                {
                    idEquipa = table.Column<decimal>(type: "numeric", nullable: false),
                    nome = table.Column<string>(type: "text", nullable: false),
                    NHabitualHoras = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipas", x => x.idEquipa);
                });

            migrationBuilder.CreateTable(
                name: "Utilizadores",
                columns: table => new
                {
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    NHabitualHoras = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilizadores", x => x.IdUtilizador);
                });

            migrationBuilder.CreateTable(
                name: "CargoUtilizador",
                columns: table => new
                {
                    CargosIdCargo = table.Column<decimal>(type: "numeric", nullable: false),
                    UtilizadoresIdUtilizador = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoUtilizador", x => new { x.CargosIdCargo, x.UtilizadoresIdUtilizador });
                    table.ForeignKey(
                        name: "FK_CargoUtilizador_Cargos_CargosIdCargo",
                        column: x => x.CargosIdCargo,
                        principalTable: "Cargos",
                        principalColumn: "IdCargo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CargoUtilizador_Utilizadores_UtilizadoresIdUtilizador",
                        column: x => x.UtilizadoresIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Convites",
                columns: table => new
                {
                    IdMensagem = table.Column<decimal>(type: "numeric", nullable: false),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    Resposta = table.Column<bool>(type: "boolean", nullable: false),
                    IdUtilizadorRemetente = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorNavigationIdUtilizador = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convites", x => x.IdMensagem);
                    table.ForeignKey(
                        name: "FK_Convites_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                        column: x => x.IdUtilizadorNavigationIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador");
                });

            migrationBuilder.CreateTable(
                name: "Projetos",
                columns: table => new
                {
                    IdProjeto = table.Column<decimal>(type: "numeric", nullable: false),
                    NomeProjeto = table.Column<string>(type: "text", nullable: false),
                    NomeCliente = table.Column<string>(type: "text", nullable: false),
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorNavigationIdUtilizador = table.Column<decimal>(type: "numeric", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projetos", x => x.IdProjeto);
                    table.ForeignKey(
                        name: "FK_Projetos_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                        column: x => x.IdUtilizadorNavigationIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador");
                });

            migrationBuilder.CreateTable(
                name: "Membros",
                columns: table => new
                {
                    IdMembro = table.Column<decimal>(type: "numeric", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    NHabitualHoras = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    IdProjeto = table.Column<decimal>(type: "numeric", nullable: false),
                    IdProjetoNavigationIdProjeto = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorNavigationIdUtilizador = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Membros", x => x.IdMembro);
                    table.ForeignKey(
                        name: "FK_Membros_Projetos_IdProjetoNavigationIdProjeto",
                        column: x => x.IdProjetoNavigationIdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Membros_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                        column: x => x.IdUtilizadorNavigationIdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tarefas",
                columns: table => new
                {
                    IdTarefa = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NomeTarefa = table.Column<string>(type: "text", nullable: false),
                    DtInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    HrInicio = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DtFim = table.Column<DateOnly>(type: "date", nullable: true),
                    HrFim = table.Column<TimeSpan>(type: "interval", nullable: false),
                    PrecoHora = table.Column<decimal>(type: "numeric", nullable: true),
                    IdProjeto = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarefas", x => x.IdTarefa);
                    table.ForeignKey(
                        name: "FK_Tarefas_Projetos_IdProjeto",
                        column: x => x.IdProjeto,
                        principalTable: "Projetos",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjetoTarefa",
                columns: table => new
                {
                    ProjetoId = table.Column<decimal>(type: "numeric", nullable: false),
                    TarefaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjetoTarefa", x => new { x.ProjetoId, x.TarefaId });
                    table.ForeignKey(
                        name: "FK_ProjetoTarefa_Projetos_ProjetoId",
                        column: x => x.ProjetoId,
                        principalTable: "Projetos",
                        principalColumn: "IdProjeto",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjetoTarefa_Tarefas_TarefaId",
                        column: x => x.TarefaId,
                        principalTable: "Tarefas",
                        principalColumn: "IdTarefa",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargoUtilizador_UtilizadoresIdUtilizador",
                table: "CargoUtilizador",
                column: "UtilizadoresIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdUtilizadorNavigationIdUtilizador",
                table: "Convites",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdProjetoNavigationIdProjeto",
                table: "Membros",
                column: "IdProjetoNavigationIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdUtilizadorNavigationIdUtilizador",
                table: "Membros",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_ProjetoTarefa_TarefaId",
                table: "ProjetoTarefa",
                column: "TarefaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_IdProjeto",
                table: "Tarefas",
                column: "IdProjeto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoUtilizador");

            migrationBuilder.DropTable(
                name: "Convites");

            migrationBuilder.DropTable(
                name: "Equipas");

            migrationBuilder.DropTable(
                name: "Membros");

            migrationBuilder.DropTable(
                name: "ProjetoTarefa");

            migrationBuilder.DropTable(
                name: "Cargos");

            migrationBuilder.DropTable(
                name: "Tarefas");

            migrationBuilder.DropTable(
                name: "Projetos");

            migrationBuilder.DropTable(
                name: "Utilizadores");
        }
    }
}
