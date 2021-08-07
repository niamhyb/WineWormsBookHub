using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DomainModel.Migrations
{
    public partial class catalogue39 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Discriminator",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "BookTable",
                columns: table => new
                {
                    BookID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ISBN = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Genres = table.Column<int>(type: "int", nullable: false),
                    AvgRating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookTable", x => x.BookID);
                });

            migrationBuilder.CreateTable(
                name: "catalogues",
                columns: table => new
                {
                    bID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookID = table.Column<int>(type: "int", nullable: true),
                    OwnerId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_catalogues", x => x.bID);
                    table.ForeignKey(
                        name: "FK_catalogues_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_catalogues_BookTable_BookID",
                        column: x => x.BookID,
                        principalTable: "BookTable",
                        principalColumn: "BookID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "loans",
                columns: table => new
                {
                    loanID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    borrowerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateLoaned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateReturned = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CataloguebID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_loans", x => x.loanID);
                    table.ForeignKey(
                        name: "FK_loans_AspNetUsers_borrowerId",
                        column: x => x.borrowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_loans_catalogues_CataloguebID",
                        column: x => x.CataloguebID,
                        principalTable: "catalogues",
                        principalColumn: "bID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "reservations",
                columns: table => new
                {
                    reservationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    borrowerId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    DateReserved = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadingOrder = table.Column<int>(type: "int", nullable: false),
                    CataloguebID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reservations", x => x.reservationID);
                    table.ForeignKey(
                        name: "FK_reservations_AspNetUsers_borrowerId",
                        column: x => x.borrowerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reservations_catalogues_CataloguebID",
                        column: x => x.CataloguebID,
                        principalTable: "catalogues",
                        principalColumn: "bID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_catalogues_BookID",
                table: "catalogues",
                column: "BookID");

            migrationBuilder.CreateIndex(
                name: "IX_catalogues_OwnerId",
                table: "catalogues",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_loans_borrowerId",
                table: "loans",
                column: "borrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_loans_CataloguebID",
                table: "loans",
                column: "CataloguebID");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_borrowerId",
                table: "reservations",
                column: "borrowerId");

            migrationBuilder.CreateIndex(
                name: "IX_reservations_CataloguebID",
                table: "reservations",
                column: "CataloguebID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "loans");

            migrationBuilder.DropTable(
                name: "reservations");

            migrationBuilder.DropTable(
                name: "catalogues");

            migrationBuilder.DropTable(
                name: "BookTable");

            migrationBuilder.DropColumn(
                name: "Discriminator",
                table: "AspNetUsers");
        }
    }
}
