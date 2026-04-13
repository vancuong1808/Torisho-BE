using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Refactor_Kanji_RemoveLearningContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Kanji_LearningContent_Id",
                table: "Kanji");

            migrationBuilder.DropForeignKey(
                name: "FK_Vocabulary_LearningContent_Id",
                table: "Vocabulary");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Kanji",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Kanji",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Kanji");

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
    }
}
