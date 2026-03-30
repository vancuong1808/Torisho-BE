using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Dictionary_schema_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // This migration replaces the deleted/rolled-back "Dictionary_updates" migration.
            // It assumes the DB is at the state produced by the previous migration
            // (DictionaryEntries + FlashCard tables), and moves it to:
            // entries + flashcards + entry_kanji + entry_reading + entry_definitions.

            // Drop FKs that depend on tables being renamed
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_DictionaryEntries_DictionaryEntryId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCard_Users_UserId",
                table: "FlashCard");

            migrationBuilder.DropForeignKey(
                name: "FK_Kanji_DictionaryEntries_DictionaryEntryId",
                table: "Kanji");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_DictionaryEntries_DictionaryEntryId",
                table: "Vocabulary");

            // Update PK names as part of the rename
            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashCard",
                table: "FlashCard");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DictionaryEntries",
                table: "DictionaryEntries");

            // Rename tables
            migrationBuilder.RenameTable(
                name: "FlashCard",
                newName: "flashcards");

            migrationBuilder.RenameTable(
                name: "DictionaryEntries",
                newName: "entries");

            // Rename FlashCard columns to snake_case
            migrationBuilder.RenameColumn(
                name: "Front",
                table: "flashcards",
                newName: "front");

            migrationBuilder.RenameColumn(
                name: "Back",
                table: "flashcards",
                newName: "back");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "flashcards",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "flashcards",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "IsFavorite",
                table: "flashcards",
                newName: "is_favorite");

            migrationBuilder.RenameColumn(
                name: "DictionaryEntryId",
                table: "flashcards",
                newName: "entry_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "flashcards",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_FlashCard_UserId",
                table: "flashcards",
                newName: "idx_flashcards_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_FlashCard_DictionaryEntryId",
                table: "flashcards",
                newName: "idx_flashcards_entry_id");

            // Rename DictionaryEntry columns to snake_case
            migrationBuilder.RenameColumn(
                name: "Jlpt",
                table: "entries",
                newName: "jlpt");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "entries",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "Reading",
                table: "entries",
                newName: "primary_reading");

            migrationBuilder.RenameColumn(
                name: "MeaningsJson",
                table: "entries",
                newName: "meanings_json");

            migrationBuilder.RenameColumn(
                name: "Keyword",
                table: "entries",
                newName: "primary_headword");

            migrationBuilder.RenameColumn(
                name: "ExamplesJson",
                table: "entries",
                newName: "examples_json");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "entries",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryEntries_Reading",
                table: "entries",
                newName: "idx_primary_reading");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryEntries_Keyword",
                table: "entries",
                newName: "idx_primary_headword");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryEntries_Jlpt_Keyword",
                table: "entries",
                newName: "idx_jlpt_primary_headword");

            migrationBuilder.RenameIndex(
                name: "IX_DictionaryEntries_Jlpt",
                table: "entries",
                newName: "idx_jlpt");

            // Align column definitions with configurations
            migrationBuilder.AlterColumn<string>(
                name: "front",
                table: "flashcards",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "back",
                table: "flashcards",
                type: "varchar(2048)",
                maxLength: 2048,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "flashcards",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<bool>(
                name: "is_favorite",
                table: "flashcards",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "flashcards",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "entries",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn);

            migrationBuilder.AlterColumn<string>(
                name: "primary_reading",
                table: "entries",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "primary_headword",
                table: "entries",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(200)",
                oldMaxLength: 200)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "entries",
                type: "datetime(6)",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)")
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            // Add new DictionaryEntry columns
            migrationBuilder.AddColumn<bool>(
                name: "is_common",
                table: "entries",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "raw_json",
                table: "entries",
                type: "json",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "source_id",
                table: "entries",
                type: "varchar(32)",
                maxLength: 32,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            // Re-add PKs
            migrationBuilder.AddPrimaryKey(
                name: "PK_flashcards",
                table: "flashcards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_entries",
                table: "entries",
                column: "Id");

            // Create new helper tables for accurate search
            migrationBuilder.CreateTable(
                name: "entry_definitions",
                columns: table => new
                {
                    entry_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    gloss_text = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_definitions", x => x.entry_id);
                    table.ForeignKey(
                        name: "FK_entry_definitions_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "entry_kanji",
                columns: table => new
                {
                    entry_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    kanji_text = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    is_common = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_kanji", x => new { x.entry_id, x.kanji_text });
                    table.ForeignKey(
                        name: "FK_entry_kanji_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "entry_reading",
                columns: table => new
                {
                    entry_id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    reading_text = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entry_reading", x => new { x.entry_id, x.reading_text });
                    table.ForeignKey(
                        name: "FK_entry_reading_entries_entry_id",
                        column: x => x.entry_id,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Create indexes
            migrationBuilder.CreateIndex(
                name: "idx_flashcards_is_favorite",
                table: "flashcards",
                column: "is_favorite");

            migrationBuilder.CreateIndex(
                name: "idx_entries_is_common",
                table: "entries",
                column: "is_common");

            migrationBuilder.CreateIndex(
                name: "ux_entries_source_id",
                table: "entries",
                column: "source_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_entry_definitions_entry_id",
                table: "entry_definitions",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "idx_entry_kanji_common",
                table: "entry_kanji",
                column: "is_common");

            migrationBuilder.CreateIndex(
                name: "idx_entry_kanji_entry",
                table: "entry_kanji",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "idx_kanji",
                table: "entry_kanji",
                column: "kanji_text");

            migrationBuilder.CreateIndex(
                name: "idx_entry_reading_entry",
                table: "entry_reading",
                column: "entry_id");

            migrationBuilder.CreateIndex(
                name: "idx_reading",
                table: "entry_reading",
                column: "reading_text");

            // Re-add FKs with new table names
            migrationBuilder.AddForeignKey(
                name: "FK_flashcards_Users_user_id",
                table: "flashcards",
                column: "user_id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_flashcards_entries_entry_id",
                table: "flashcards",
                column: "entry_id",
                principalTable: "entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kanji_entries_DictionaryEntryId",
                table: "Kanji",
                column: "DictionaryEntryId",
                principalTable: "entries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabulary_entries_DictionaryEntryId",
                table: "Vocabulary",
                column: "DictionaryEntryId",
                principalTable: "entries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flashcards_Users_user_id",
                table: "flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_flashcards_entries_entry_id",
                table: "flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_Kanji_entries_DictionaryEntryId",
                table: "Kanji");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_entries_DictionaryEntryId",
                table: "Vocabulary");

            migrationBuilder.DropTable(
                name: "entry_definitions");

            migrationBuilder.DropTable(
                name: "entry_kanji");

            migrationBuilder.DropTable(
                name: "entry_reading");

            migrationBuilder.DropPrimaryKey(
                name: "PK_flashcards",
                table: "flashcards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_entries",
                table: "entries");

            migrationBuilder.DropIndex(
                name: "idx_flashcards_is_favorite",
                table: "flashcards");

            migrationBuilder.DropIndex(
                name: "idx_entries_is_common",
                table: "entries");

            migrationBuilder.DropIndex(
                name: "ux_entries_source_id",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "is_common",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "raw_json",
                table: "entries");

            migrationBuilder.DropColumn(
                name: "source_id",
                table: "entries");

            migrationBuilder.RenameTable(
                name: "flashcards",
                newName: "FlashCard");

            migrationBuilder.RenameTable(
                name: "entries",
                newName: "DictionaryEntries");

            migrationBuilder.RenameColumn(
                name: "front",
                table: "FlashCard",
                newName: "Front");

            migrationBuilder.RenameColumn(
                name: "back",
                table: "FlashCard",
                newName: "Back");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "FlashCard",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "FlashCard",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "is_favorite",
                table: "FlashCard",
                newName: "IsFavorite");

            migrationBuilder.RenameColumn(
                name: "entry_id",
                table: "FlashCard",
                newName: "DictionaryEntryId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "FlashCard",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "idx_flashcards_user_id",
                table: "FlashCard",
                newName: "IX_FlashCard_UserId");

            migrationBuilder.RenameIndex(
                name: "idx_flashcards_entry_id",
                table: "FlashCard",
                newName: "IX_FlashCard_DictionaryEntryId");

            migrationBuilder.RenameColumn(
                name: "jlpt",
                table: "DictionaryEntries",
                newName: "Jlpt");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "DictionaryEntries",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "primary_reading",
                table: "DictionaryEntries",
                newName: "Reading");

            migrationBuilder.RenameColumn(
                name: "meanings_json",
                table: "DictionaryEntries",
                newName: "MeaningsJson");

            migrationBuilder.RenameColumn(
                name: "primary_headword",
                table: "DictionaryEntries",
                newName: "Keyword");

            migrationBuilder.RenameColumn(
                name: "examples_json",
                table: "DictionaryEntries",
                newName: "ExamplesJson");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "DictionaryEntries",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "idx_primary_reading",
                table: "DictionaryEntries",
                newName: "IX_DictionaryEntries_Reading");

            migrationBuilder.RenameIndex(
                name: "idx_primary_headword",
                table: "DictionaryEntries",
                newName: "IX_DictionaryEntries_Keyword");

            migrationBuilder.RenameIndex(
                name: "idx_jlpt_primary_headword",
                table: "DictionaryEntries",
                newName: "IX_DictionaryEntries_Jlpt_Keyword");

            migrationBuilder.RenameIndex(
                name: "idx_jlpt",
                table: "DictionaryEntries",
                newName: "IX_DictionaryEntries_Jlpt");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashCard",
                table: "FlashCard",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DictionaryEntries",
                table: "DictionaryEntries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_DictionaryEntries_DictionaryEntryId",
                table: "FlashCard",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCard_Users_UserId",
                table: "FlashCard",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Kanji_DictionaryEntries_DictionaryEntryId",
                table: "Kanji",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vocabulary_DictionaryEntries_DictionaryEntryId",
                table: "Vocabulary",
                column: "DictionaryEntryId",
                principalTable: "DictionaryEntries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
