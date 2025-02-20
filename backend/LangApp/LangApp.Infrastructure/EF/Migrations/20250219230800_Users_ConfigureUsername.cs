using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Users_ConfigureUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserName",
                schema: "application",
                table: "ApplicationUsers");

            migrationBuilder.RenameColumn(
                name: "Username",
                schema: "application",
                table: "ApplicationUsers",
                newName: "UserName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                schema: "application",
                table: "ApplicationUsers",
                newName: "Username");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                schema: "application",
                table: "ApplicationUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
