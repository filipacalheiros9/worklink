using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ES2_webapp.Migrations
{
    /// <inheritdoc />
    public partial class AddConviteEquipaRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEquipa",
                table: "Convites",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Convites_IdEquipa",
                table: "Convites",
                column: "IdEquipa");

            migrationBuilder.AddForeignKey(
                name: "FK_Convites_Equipas_IdEquipa",
                table: "Convites",
                column: "IdEquipa",
                principalTable: "Equipas",
                principalColumn: "IdEquipa",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Convites_Equipas_IdEquipa",
                table: "Convites");

            migrationBuilder.DropIndex(
                name: "IX_Convites_IdEquipa",
                table: "Convites");

            migrationBuilder.DropColumn(
                name: "IdEquipa",
                table: "Convites");
        }
    }
}
