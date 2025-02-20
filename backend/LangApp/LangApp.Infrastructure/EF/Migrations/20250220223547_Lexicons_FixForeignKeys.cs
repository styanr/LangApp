using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Lexicons_FixForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Definition",
                schema: "application",
                table: "LexiconEntryDefinitions",
                newName: "Value");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Value",
                schema: "application",
                table: "LexiconEntryDefinitions",
                newName: "Definition");
        }
    }
}
