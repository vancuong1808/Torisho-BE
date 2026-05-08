using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ReuseLearningProgressForDashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CurrentChapterId",
                table: "LearningProgresses",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "CurrentLessonId",
                table: "LearningProgresses",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<float>(
                name: "CurrentLessonProgressPercent",
                table: "LearningProgresses",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "CurrentSection",
                table: "LearningProgresses",
                type: "varchar(100)",
                maxLength: 100,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentChapterId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "CurrentLessonId",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "CurrentLessonProgressPercent",
                table: "LearningProgresses");

            migrationBuilder.DropColumn(
                name: "CurrentSection",
                table: "LearningProgresses");
        }
    }
}
