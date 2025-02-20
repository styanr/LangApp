using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LangApp.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class Init_Read : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "application");

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false),
                    PictureUrl = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Lexicons",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lexicons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lexicons_ApplicationUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "application",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudyGroups",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Language = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudyGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudyGroups_ApplicationUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalSchema: "application",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LexiconEntries",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LexiconId = table.Column<Guid>(type: "uuid", nullable: false),
                    Term = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LexiconEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LexiconEntries_Lexicons_LexiconId",
                        column: x => x.LexiconId,
                        principalSchema: "application",
                        principalTable: "Lexicons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                schema: "application",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => new { x.UserId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_Members_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "application",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Members_StudyGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "application",
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uuid", nullable: false),
                    GroupId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Content = table.Column<string>(type: "text", nullable: false),
                    EditedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Media = table.Column<string[]>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_ApplicationUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalSchema: "application",
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_StudyGroups_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "application",
                        principalTable: "StudyGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LexiconEntryDefinitions",
                schema: "application",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LexiconEntryId = table.Column<Guid>(type: "uuid", nullable: false),
                    Definition = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LexiconEntryDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LexiconEntryDefinitions_LexiconEntries_LexiconEntryId",
                        column: x => x.LexiconEntryId,
                        principalSchema: "application",
                        principalTable: "LexiconEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LexiconEntries_LexiconId",
                schema: "application",
                table: "LexiconEntries",
                column: "LexiconId");

            migrationBuilder.CreateIndex(
                name: "IX_LexiconEntryDefinitions_LexiconEntryId",
                schema: "application",
                table: "LexiconEntryDefinitions",
                column: "LexiconEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Lexicons_OwnerId",
                schema: "application",
                table: "Lexicons",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_GroupId",
                schema: "application",
                table: "Members",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                schema: "application",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_GroupId",
                schema: "application",
                table: "Posts",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudyGroups_OwnerId",
                schema: "application",
                table: "StudyGroups",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LexiconEntryDefinitions",
                schema: "application");

            migrationBuilder.DropTable(
                name: "Members",
                schema: "application");

            migrationBuilder.DropTable(
                name: "Posts",
                schema: "application");

            migrationBuilder.DropTable(
                name: "LexiconEntries",
                schema: "application");

            migrationBuilder.DropTable(
                name: "StudyGroups",
                schema: "application");

            migrationBuilder.DropTable(
                name: "Lexicons",
                schema: "application");

            migrationBuilder.DropTable(
                name: "ApplicationUsers",
                schema: "application");
        }
    }
}
