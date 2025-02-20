using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Users_Email : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "application",
                table: "ApplicationUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                schema: "application",
                table: "ApplicationUsers");
        }
    }
}
