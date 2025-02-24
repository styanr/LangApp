using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Lexicons_FixNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lexicons_ApplicationUsers_OwnerId",
                schema: "application",
                table: "Lexicons");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                schema: "application",
                table: "Lexicons",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Lexicons_OwnerId",
                schema: "application",
                table: "Lexicons",
                newName: "IX_Lexicons_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lexicons_ApplicationUsers_UserId",
                schema: "application",
                table: "Lexicons",
                column: "UserId",
                principalSchema: "application",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lexicons_ApplicationUsers_UserId",
                schema: "application",
                table: "Lexicons");

            migrationBuilder.RenameColumn(
                name: "UserId",
                schema: "application",
                table: "Lexicons",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Lexicons_UserId",
                schema: "application",
                table: "Lexicons",
                newName: "IX_Lexicons_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lexicons_ApplicationUsers_OwnerId",
                schema: "application",
                table: "Lexicons",
                column: "OwnerId",
                principalSchema: "application",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
