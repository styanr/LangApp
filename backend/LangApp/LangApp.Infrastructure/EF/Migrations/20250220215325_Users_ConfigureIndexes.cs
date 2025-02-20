using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Users_ConfigureIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_Email",
                schema: "application",
                table: "ApplicationUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_UserName",
                schema: "application",
                table: "ApplicationUsers",
                column: "UserName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_Email",
                schema: "application",
                table: "ApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsers_UserName",
                schema: "application",
                table: "ApplicationUsers");
        }
    }
}
