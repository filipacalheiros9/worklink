using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication2.Migrations
{
    /// <inheritdoc />
    public partial class SetIdUtilizadorAutoIncrement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Projetos_IdProjeto",
                table: "Membros");

            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Utilizadores_IdUtilizador",
                table: "Membros");

            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizador",
                table: "Projetos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Projetos_IdProjeto",
                table: "Tarefas");

            migrationBuilder.AddColumn<decimal>(
                name: "IdProjetoNavigationIdProjeto",
                table: "Tarefas",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IdProjetoNavigationIdProjeto",
                table: "Membros",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Membros",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_Tarefas_IdProjetoNavigationIdProjeto",
                table: "Tarefas",
                column: "IdProjetoNavigationIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdProjetoNavigationIdProjeto",
                table: "Membros",
                column: "IdProjetoNavigationIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdUtilizadorNavigationIdUtilizador",
                table: "Membros",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Projetos_IdProjetoNavigationIdProjeto",
                table: "Membros",
                column: "IdProjetoNavigationIdProjeto",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Membros",
                column: "IdUtilizadorNavigationIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos",
                column: "IdUtilizadorNavigationIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefas_Projetos_IdProjetoNavigationIdProjeto",
                table: "Tarefas",
                column: "IdProjetoNavigationIdProjeto",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Projetos_IdProjetoNavigationIdProjeto",
                table: "Membros");

            migrationBuilder.DropForeignKey(
                name: "FK_Membros_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Membros");

            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefas_Projetos_IdProjetoNavigationIdProjeto",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Tarefas_IdProjetoNavigationIdProjeto",
                table: "Tarefas");

            migrationBuilder.DropIndex(
                name: "IX_Projetos_IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.DropIndex(
                name: "IX_Membros_IdProjetoNavigationIdProjeto",
                table: "Membros");

            migrationBuilder.DropIndex(
                name: "IX_Membros_IdUtilizadorNavigationIdUtilizador",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "IdProjetoNavigationIdProjeto",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "IdProjetoNavigationIdProjeto",
                table: "Membros");

            migrationBuilder.DropColumn(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Membros");

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Projetos_IdProjeto",
                table: "Membros",
                column: "IdProjeto",
                principalTable: "Projetos",
                principalColumn: "IdProjeto",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Membros_Utilizadores_IdUtilizador",
                table: "Membros",
                column: "IdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Utilizadores_IdUtilizador",
                table: "Projetos",
                column: "IdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
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
