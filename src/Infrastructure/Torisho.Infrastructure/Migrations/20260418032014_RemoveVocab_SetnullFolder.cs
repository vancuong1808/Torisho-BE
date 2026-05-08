using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVocab_SetnullFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flashcard_decks_flashcard_folders_folder_id",
                table: "flashcard_decks");

            migrationBuilder.DropTable(
                name: "VideoLessonVocabularies");

            migrationBuilder.DropTable(
                name: "Vocabulary");

            migrationBuilder.AddForeignKey(
                name: "FK_flashcard_decks_flashcard_folders_folder_id",
                table: "flashcard_decks",
                column: "folder_id",
                principalTable: "flashcard_folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flashcard_decks_flashcard_folders_folder_id",
                table: "flashcard_decks");

            migrationBuilder.CreateTable(
                name: "Vocabulary",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    DictionaryEntryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vocabulary", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vocabulary_entries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VideoLessonVocabularies",
                columns: table => new
                {
                    VideoLessonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    VocabulariesId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideoLessonVocabularies", x => new { x.VideoLessonId, x.VocabulariesId });
                    table.ForeignKey(
                        name: "FK_VideoLessonVocabularies_VideoLessons_VideoLessonId",
                        column: x => x.VideoLessonId,
                        principalTable: "VideoLessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideoLessonVocabularies_Vocabulary_VocabulariesId",
                        column: x => x.VocabulariesId,
                        principalTable: "Vocabulary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_VideoLessonVocabularies_VocabulariesId",
                table: "VideoLessonVocabularies",
                column: "VocabulariesId");

            migrationBuilder.CreateIndex(
                name: "IX_Vocabulary_DictionaryEntryId",
                table: "Vocabulary",
                column: "DictionaryEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_flashcard_decks_flashcard_folders_folder_id",
                table: "flashcard_decks",
                column: "folder_id",
                principalTable: "flashcard_folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
