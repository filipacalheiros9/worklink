using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES2_webapp.Migrations
{
    /// <inheritdoc />
    public partial class CorrigirChaveConvites : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Convites_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Convites");

            migrationBuilder.DropTable(
                name: "CargoUtilizador");

            migrationBuilder.DropTable(
                name: "Membros");

            migrationBuilder.DropIndex(
                name: "IX_Convites_IdUtilizadorNavigationIdUtilizador",
                table: "Convites");

            migrationBuilder.DropColumn(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Convites");

            migrationBuilder.AddColumn<decimal>(
                name: "CargoId",
                table: "Utilizadores",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UtilizadorIdUtilizador",
                table: "Cargos",
                type: "numeric",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_CargoId",
                table: "Utilizadores",
                column: "CargoId");

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdUtilizador",
                table: "Convites",
                column: "IdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Cargos_UtilizadorIdUtilizador",
                table: "Cargos",
                column: "UtilizadorIdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Cargos_Utilizadores_UtilizadorIdUtilizador",
                table: "Cargos",
                column: "UtilizadorIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Convites_Utilizadores_IdUtilizador",
                table: "Convites",
                column: "IdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Cargos_CargoId",
                table: "Utilizadores",
                column: "CargoId",
                principalTable: "Cargos",
                principalColumn: "IdCargo",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cargos_Utilizadores_UtilizadorIdUtilizador",
                table: "Cargos");

            migrationBuilder.DropForeignKey(
                name: "FK_Convites_Utilizadores_IdUtilizador",
                table: "Convites");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Cargos_CargoId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_CargoId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Convites_IdUtilizador",
                table: "Convites");

            migrationBuilder.DropIndex(
                name: "IX_Cargos_UtilizadorIdUtilizador",
                table: "Cargos");

            migrationBuilder.DropColumn(
                name: "CargoId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "UtilizadorIdUtilizador",
                table: "Cargos");

            migrationBuilder.AddColumn<decimal>(
                name: "IdUtilizadorNavigationIdUtilizador",
                table: "Convites",
                type: "numeric",
                nullable: true);

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
                name: "Membros",
                columns: table => new
                {
                    IdMembro = table.Column<decimal>(type: "numeric", nullable: false),
                    IdProjetoNavigationIdProjeto = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizadorNavigationIdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    IdProjeto = table.Column<decimal>(type: "numeric", nullable: false),
                    IdUtilizador = table.Column<decimal>(type: "numeric", nullable: false),
                    NHabitualHoras = table.Column<decimal>(type: "numeric", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdUtilizadorNavigationIdUtilizador",
                table: "Convites",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_CargoUtilizador_UtilizadoresIdUtilizador",
                table: "CargoUtilizador",
                column: "UtilizadoresIdUtilizador");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdProjetoNavigationIdProjeto",
                table: "Membros",
                column: "IdProjetoNavigationIdProjeto");

            migrationBuilder.CreateIndex(
                name: "IX_Membros_IdUtilizadorNavigationIdUtilizador",
                table: "Membros",
                column: "IdUtilizadorNavigationIdUtilizador");

            migrationBuilder.AddForeignKey(
                name: "FK_Convites_Utilizadores_IdUtilizadorNavigationIdUtilizador",
                table: "Convites",
                column: "IdUtilizadorNavigationIdUtilizador",
                principalTable: "Utilizadores",
                principalColumn: "IdUtilizador");
        }
    }
}
