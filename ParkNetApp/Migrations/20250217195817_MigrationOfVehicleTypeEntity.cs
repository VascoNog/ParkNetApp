using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkNetApp.Migrations
{
    /// <inheritdoc />
    public partial class MigrationOfVehicleTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(20)", nullable: false),
                    Symbol = table.Column<string>(type: "nchar(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_Symbol",
                table: "VehicleTypes",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VehicleTypes_Type",
                table: "VehicleTypes",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleTypes");
        }
    }
}
