using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkNetApp.Migrations
{
    /// <inheritdoc />
    public partial class CreateEmailBoxEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailBoxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RecipientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailBoxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmailBoxes_AspNetUsers_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmailBoxes_RecipientId",
                table: "EmailBoxes",
                column: "RecipientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailBoxes");
        }
    }
}
