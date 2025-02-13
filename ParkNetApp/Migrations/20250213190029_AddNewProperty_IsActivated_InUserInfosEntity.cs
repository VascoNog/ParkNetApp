using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkNetApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewProperty_IsActivated_InUserInfosEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActivated",
                table: "UserInfos",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActivated",
                table: "UserInfos");
        }
    }
}
