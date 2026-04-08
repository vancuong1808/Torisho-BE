using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixKanjiModel_Entry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kanji_entries_DictionaryEntryId",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "IX_Kanji_DictionaryEntryId",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "DictionaryEntryId",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "Meaning",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "StrokeOrderGifUrl",
                table: "Kanji");

            migrationBuilder.RenameColumn(
                name: "OnYomi",
                table: "Kanji",
                newName: "Onyomi");

            migrationBuilder.RenameColumn(
                name: "KunYomi",
                table: "Kanji",
                newName: "Kunyomi");

            migrationBuilder.AlterColumn<string>(
                name: "Onyomi",
                table: "Kanji",
                type: "varchar(256)",
                maxLength: 256,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Kunyomi",
                table: "Kanji",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Character",
                table: "Kanji",
                type: "varchar(1)",
                maxLength: 1,
                nullable: false,
                collation: "utf8mb4_bin",
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Frequency",
                table: "Kanji",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Kanji",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JlptLevel",
                table: "Kanji",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeaningsJson",
                table: "Kanji",
                type: "json",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Kanji",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UnicodeHex",
                table: "Kanji",
                type: "varchar(16)",
                maxLength: 16,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "entry_kanji_map",
                columns: table => new
                {
                    entry_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    kanji_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    char_position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_kanji_map", x => new { x.entry_id, x.kanji_id, x.char_position });
                    table.ForeignKey(
                        name: "FK_entry_kanji_map_Kanji_kanji_id",
                        column: x => x.kanji_id,
                        principalTable: "Kanji",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_entry_kanji_map_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "idx_kanji_freq",
                table: "Kanji",
                column: "Frequency");

            migrationBuilder.CreateIndex(
                name: "idx_kanji_grade",
                table: "Kanji",
                column: "Grade");

            migrationBuilder.CreateIndex(
                name: "idx_kanji_jlpt",
                table: "Kanji",
                column: "JlptLevel");

            migrationBuilder.CreateIndex(
                name: "idx_kanji_type",
                table: "Kanji",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "idx_kanji_ucs",
                table: "Kanji",
                column: "UnicodeHex");

            migrationBuilder.CreateIndex(
                name: "ux_kanji_character",
                table: "Kanji",
                column: "Character",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_entry_kanji_map_entry",
                table: "entry_kanji_map",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "idx_entry_kanji_map_kanji",
                table: "entry_kanji_map",
                column: "kanji_id");

            migrationBuilder.CreateIndex(
                name: "idx_entry_kanji_map_lookup",
                table: "entry_kanji_map",
                columns: new[] { "kanji_id", "entry_id" });

            migrationBuilder.AddForeignKey(
                name: "FK_Kanji_LearningContent_Id",
                table: "Kanji",
                column: "Id",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabulary_LearningContent_Id",
                table: "Vocabulary",
                column: "Id",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kanji_LearningContent_Id",
                table: "Kanji");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_LearningContent_Id",
                table: "Vocabulary");

            migrationBuilder.DropTable(
                name: "entry_kanji_map");

            migrationBuilder.DropIndex(
                name: "idx_kanji_freq",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "idx_kanji_grade",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "idx_kanji_jlpt",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "idx_kanji_type",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "idx_kanji_ucs",
                table: "Kanji");

            migrationBuilder.DropIndex(
                name: "ux_kanji_character",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "JlptLevel",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "MeaningsJson",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "UnicodeHex",
                table: "Kanji");

            migrationBuilder.RenameColumn(
                name: "Onyomi",
                table: "Kanji",
                newName: "OnYomi");

            migrationBuilder.RenameColumn(
                name: "Kunyomi",
                table: "Kanji",
                newName: "KunYomi");

            migrationBuilder.AlterColumn<string>(
                name: "OnYomi",
                table: "Kanji",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(256)",
                oldMaxLength: 256)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "KunYomi",
                table: "Kanji",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Character",
                table: "Kanji",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(1)",
                oldMaxLength: 1,
                oldCollation: "utf8mb4_bin")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "DictionaryEntryId",
                table: "Kanji",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Meaning",
                table: "Kanji",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StrokeOrderGifUrl",
                table: "Kanji",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Kanji_DictionaryEntryId",
                table: "Kanji",
                column: "DictionaryEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Kanji_entries_DictionaryEntryId",
                table: "Kanji",
                column: "DictionaryEntryId",
                principalTable: "entries",
                principalColumn: "Id");
        }
    }
}
