using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES2_webapp.Migrations
{
    /// <inheritdoc />
    public partial class AddProjetoTarefaRelationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjetoTarefa_Projetos_ProjetoId",
                table: "ProjetoTarefa");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Projetos_IdProjeto",
                table: "Tarefas");

            migrationBuilder.DropTable(
                name: "Convites");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_IdProjeto",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Projetos_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "IdProjeto",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.RenameColumn(
                name: "ProjetoId",
                table: "ProjetoTarefa",
                newName: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdUtilizador",
                table: "Projetos",
                column: "IdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizador",
                table: "Projetos",
                column: "IdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjetoTarefa_Projetos_IdProjeto",
                table: "ProjetoTarefa",
                column: "IdProjeto",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizador",
                table: "Projetos");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjetoTarefa_Projetos_IdProjeto",
                table: "ProjetoTarefa");

            migrationBuilder.DropIndex(
                name: "IX_Projetos_IdUtilizador",
                table: "Projetos");

            migrationBuilder.RenameColumn(
                name: "IdProjeto",
                table: "ProjetoTarefa",
                newName: "ProjetoId");

            migrationBuilder.AddColumn<decimal>(
                name: "IdProjeto",
                table: "Tarefas",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Convites",
                columns: table => new
                {
                    IdMensagem = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorRemetente = table.Column<decimal>(type: "numeric", nullable: false),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    Resposta = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convites", x => x.IdMensagem);
                    table.ForeignKey(
                        name: "FK_Convites_Utilizadores_IdUtilizador",
                        column: x => x.IdUtilizador,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_IdProjeto",
                table: "Tarefas",
                column: "IdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdUtilizador",
                table: "Convites",
                column: "IdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                column: "IdUtilizadorNavigationIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjetoTarefa_Projetos_ProjetoId",
                table: "ProjetoTarefa",
                column: "ProjetoId",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Projetos_IdProjeto",
                table: "Tarefas",
                column: "IdProjeto",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
