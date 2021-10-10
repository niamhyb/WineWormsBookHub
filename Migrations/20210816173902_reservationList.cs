using Microsoft.EntityFrameworkCore.Migrations;

namespace DomainModel.Migrations
{
    public partial class reservationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_catalogues_CataloguebID",
                table: "reservations");

            migrationBuilder.RenameColumn(
                name: "CataloguebID",
                table: "reservations",
                newName: "cataloguebID");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_CataloguebID",
                table: "reservations",
                newName: "IX_reservations_cataloguebID");

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_catalogues_cataloguebID",
                table: "reservations",
                column: "cataloguebID",
                principalTable: "catalogues",
                principalColumn: "bID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reservations_catalogues_cataloguebID",
                table: "reservations");

            migrationBuilder.RenameColumn(
                name: "cataloguebID",
                table: "reservations",
                newName: "CataloguebID");

            migrationBuilder.RenameIndex(
                name: "IX_reservations_cataloguebID",
                table: "reservations",
                newName: "IX_reservations_CataloguebID");

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
