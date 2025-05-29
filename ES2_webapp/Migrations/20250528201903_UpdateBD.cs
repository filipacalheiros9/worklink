using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ES2_webapp.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "nome",
                table: "Equipas",
                newName: "Nome");

            migrationBuilder.RenameColumn(
                name: "idEquipa",
                table: "Equipas",
                newName: "IdEquipa");

            migrationBuilder.AddColumn<decimal>(
                name: "IdUtilizador",
                table: "Tarefas",
                type: "numeric",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdProjeto",
                table: "ProjetoTarefa",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric");

            migrationBuilder.AlterColumn<int>(
                name: "IdProjeto",
                table: "Projetos",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<int>(
                name: "EquipaId",
                table: "Projetos",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEquipa",
                table: "Equipas",
                type: "integer",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<decimal>(
                name: "IdCriador",
                table: "Equipas",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "Convites",
                columns: table => new
                {
                    IdMensagem = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Mensagem = table.Column<string>(type: "text", nullable: false),
                    Resposta = table.Column<bool>(type: "boolean", nullable: true),
                    FoiLido = table.Column<bool>(type: "boolean", nullable: false),
                    IdUtilizadorRemetente = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorDestinatario = table.Column<decimal>(type: "numeric", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Convites", x => x.IdMensagem);
                    table.ForeignKey(
                        name: "FK_Convites_Utilizadores_IdUtilizadorDestinatario",
                        column: x => x.IdUtilizadorDestinatario,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EquipaUtilizadores",
                columns: table => new
                {
                    EquipaId = table.Column<int>(type: "integer", nullable: false),
                    UtilizadorId = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EquipaUtilizadores", x => new { x.EquipaId, x.UtilizadorId });
                    table.ForeignKey(
                        name: "FK_EquipaUtilizadores_Equipas_EquipaId",
                        column: x => x.EquipaId,
                        principalTable: "Equipas",
                        principalColumn: "IdEquipa",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EquipaUtilizadores_Utilizadores_UtilizadorId",
                        column: x => x.UtilizadorId,
                        principalTable: "Utilizadores",
                        principalColumn: "IdUtilizador",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projetos_EquipaId",
                table: "Projetos",
                column: "EquipaId");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdUtilizadorDestinatario",
                table: "Convites",
                column: "IdUtilizadorDestinatario");

            migrationBuilder.CreateIndex(
                name: "IX_EquipaUtilizadores_UtilizadorId",
                table: "EquipaUtilizadores",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Projetos_Equipas_EquipaId",
                table: "Projetos",
                column: "EquipaId",
                principalTable: "Equipas",
                principalColumn: "IdEquipa",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projetos_Equipas_EquipaId",
                table: "Projetos");

            migrationBuilder.DropTable(
                name: "Convites");

            migrationBuilder.DropTable(
                name: "EquipaUtilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Projetos_EquipaId",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "IdUtilizador",
                table: "Tarefas");

            migrationBuilder.DropColumn(
                name: "EquipaId",
                table: "Projetos");

            migrationBuilder.DropColumn(
                name: "IdCriador",
                table: "Equipas");

            migrationBuilder.RenameColumn(
                name: "Nome",
                table: "Equipas",
                newName: "nome");

            migrationBuilder.RenameColumn(
                name: "IdEquipa",
                table: "Equipas",
                newName: "idEquipa");

            migrationBuilder.AlterColumn<decimal>(
                name: "IdProjeto",
                table: "ProjetoTarefa",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<decimal>(
                name: "IdProjeto",
                table: "Projetos",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AlterColumn<decimal>(
                name: "idEquipa",
                table: "Equipas",
                type: "numeric",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
        }
    }
}
