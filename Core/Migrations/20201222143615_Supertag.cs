using Microsoft.EntityFrameworkCore.Migrations;

namespace Core.Migrations
{
    public partial class Supertag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NoteTag");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Tags",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SuperTagKey",
                table: "Tags",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "NoteTags",
                columns: table => new
                {
                    NoteKey = table.Column<int>(nullable: false),
                    TagKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTags", x => new { x.NoteKey, x.TagKey });
                    table.ForeignKey(
                        name: "FK_NoteTags_Notes_NoteKey",
                        column: x => x.NoteKey,
                        principalTable: "Notes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteTags_Tags_TagKey",
                        column: x => x.TagKey,
                        principalTable: "Tags",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SuperTags",
                columns: table => new
                {
                    Key = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SuperTags", x => x.Key);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_SuperTagKey",
                table: "Tags",
                column: "SuperTagKey");

            migrationBuilder.CreateIndex(
                name: "IX_Notes_Name",
                table: "Notes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NoteTags_TagKey",
                table: "NoteTags",
                column: "TagKey");

            migrationBuilder.CreateIndex(
                name: "IX_SuperTags_Name",
                table: "SuperTags",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_SuperTags_SuperTagKey",
                table: "Tags",
                column: "SuperTagKey",
                principalTable: "SuperTags",
                principalColumn: "Key",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_SuperTags_SuperTagKey",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "NoteTags");

            migrationBuilder.DropTable(
                name: "SuperTags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_SuperTagKey",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Notes_Name",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "SuperTagKey",
                table: "Tags");

            migrationBuilder.CreateTable(
                name: "NoteTag",
                columns: table => new
                {
                    NoteKey = table.Column<int>(type: "INTEGER", nullable: false),
                    TagKey = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NoteTag", x => new { x.NoteKey, x.TagKey });
                    table.ForeignKey(
                        name: "FK_NoteTag_Notes_NoteKey",
                        column: x => x.NoteKey,
                        principalTable: "Notes",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NoteTag_Tags_TagKey",
                        column: x => x.TagKey,
                        principalTable: "Tags",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NoteTag_TagKey",
                table: "NoteTag",
                column: "TagKey");
        }
    }
}
