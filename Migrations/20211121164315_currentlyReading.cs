using Microsoft.EntityFrameworkCore.Migrations;

namespace DomainModel.Migrations
{
    public partial class currentlyReading : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_catalogues_CataloguebID",
                table: "loans");

            migrationBuilder.RenameColumn(
                name: "CataloguebID",
                table: "loans",
                newName: "cataloguebID");

            migrationBuilder.RenameIndex(
                name: "IX_loans_CataloguebID",
                table: "loans",
                newName: "IX_loans_cataloguebID");

            migrationBuilder.AddColumn<int>(
                name: "loanID",
                table: "reservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_reservations_loanID",
                table: "reservations",
                column: "loanID");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_catalogues_cataloguebID",
                table: "loans",
                column: "cataloguebID",
                principalTable: "catalogues",
                principalColumn: "bID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reservations_loans_loanID",
                table: "reservations",
                column: "loanID",
                principalTable: "loans",
                principalColumn: "loanID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_loans_catalogues_cataloguebID",
                table: "loans");

            migrationBuilder.DropForeignKey(
                name: "FK_reservations_loans_loanID",
                table: "reservations");

            migrationBuilder.DropIndex(
                name: "IX_reservations_loanID",
                table: "reservations");

            migrationBuilder.DropColumn(
                name: "loanID",
                table: "reservations");

            migrationBuilder.RenameColumn(
                name: "cataloguebID",
                table: "loans",
                newName: "CataloguebID");

            migrationBuilder.RenameIndex(
                name: "IX_loans_cataloguebID",
                table: "loans",
                newName: "IX_loans_CataloguebID");

            migrationBuilder.AddForeignKey(
                name: "FK_loans_catalogues_CataloguebID",
                table: "loans",
                column: "CataloguebID",
                principalTable: "catalogues",
                principalColumn: "bID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
