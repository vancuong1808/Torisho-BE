using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LessonCurriculumSchemaV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Lessons",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContentId",
                table: "Lessons",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Lessons",
                type: "varchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SourceLevel",
                table: "Lessons",
                type: "varchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.Sql(@"
                UPDATE Lessons l
                INNER JOIN Chapters c ON c.Id = l.ChapterId
                INNER JOIN Levels lv ON lv.Id = c.LevelId
                SET l.SourceLevel = lv.Code
                WHERE l.SourceLevel IS NULL OR l.SourceLevel = '';
            ");

            migrationBuilder.Sql(@"
                UPDATE Lessons
                SET Slug = CONCAT('legacy-', REPLACE(CAST(Id AS CHAR(36)), '-', ''))
                WHERE Slug IS NULL OR Slug = '';
            ");

            migrationBuilder.CreateTable(
                name: "LessonGrammarItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LessonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    GrammarPoint = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeaningEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DetailUrl = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LevelHint = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Explanation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UsageJson = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamplesJson = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonGrammarItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonGrammarItems_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LessonReadingItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LessonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Content = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Translation = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Url = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LevelHint = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Source = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonReadingItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonReadingItems_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LessonVocabularyItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LessonId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    SortOrder = table.Column<int>(type: "int", nullable: false),
                    Term = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Reading = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MeaningsJson = table.Column<string>(type: "json", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExamplesJson = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OtherFormsJson = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCommon = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    JlptTagsJson = table.Column<string>(type: "json", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonVocabularyItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonVocabularyItems_Lessons_LessonId",
                        column: x => x.LessonId,
                        principalTable: "Lessons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "UX_Lessons_Slug",
                table: "Lessons",
                column: "Slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonGrammarItems_LessonId",
                table: "LessonGrammarItems",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "UX_LessonGrammarItems_LessonId_SortOrder",
                table: "LessonGrammarItems",
                columns: new[] { "LessonId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonReadingItems_LessonId",
                table: "LessonReadingItems",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "UX_LessonReadingItems_LessonId_SortOrder",
                table: "LessonReadingItems",
                columns: new[] { "LessonId", "SortOrder" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonVocabularyItems_LessonId",
                table: "LessonVocabularyItems",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonVocabularyItems_Term",
                table: "LessonVocabularyItems",
                column: "Term");

            migrationBuilder.CreateIndex(
                name: "UX_LessonVocabularyItems_LessonId_SortOrder",
                table: "LessonVocabularyItems",
                columns: new[] { "LessonId", "SortOrder" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons",
                column: "ContentId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "LessonGrammarItems");

            migrationBuilder.DropTable(
                name: "LessonReadingItems");

            migrationBuilder.DropTable(
                name: "LessonVocabularyItems");

            migrationBuilder.DropIndex(
                name: "UX_Lessons_Slug",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "SourceLevel",
                table: "Lessons");

            migrationBuilder.UpdateData(
                table: "Lessons",
                keyColumn: "Type",
                keyValue: null,
                column: "Type",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Lessons",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "ContentId",
                table: "Lessons",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci",
                oldClrType: typeof(Guid),
                oldType: "char(36)",
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_LearningContent_ContentId",
                table: "Lessons",
                column: "ContentId",
                principalTable: "LearningContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
