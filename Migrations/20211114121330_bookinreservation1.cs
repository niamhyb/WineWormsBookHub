using Microsoft.EntityFrameworkCore.Migrations;

namespace DomainModel.Migrations
{
    public partial class bookinreservation1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_catalogues_CataloguebID",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_reservations_CataloguebID",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "CataloguebID",
                table: "reservations");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CataloguebID",
                table: "reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_CataloguebID",
                table: "reservations",
                column: "CataloguebID");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_catalogues_CataloguebID",
                table: "reservations",
                column: "CataloguebID",
                principalTable: "catalogues",
                principalColumn: "bID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
