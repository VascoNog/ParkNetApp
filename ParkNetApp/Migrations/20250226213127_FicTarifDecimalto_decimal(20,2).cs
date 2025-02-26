using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkNetApp.Migrations
{
    /// <inheritdoc />
    public partial class FicTarifDecimalto_decimal202 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Tariff",
                table: "NonSubscriptionParkingTariffs",
                type: "decimal(20,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Limit",
                table: "NonSubscriptionParkingTariffs",
                type: "decimal(20,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Tariff",
                table: "NonSubscriptionParkingTariffs",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Limit",
                table: "NonSubscriptionParkingTariffs",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(20,2)");
        }
    }
}
