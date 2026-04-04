using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDictionaryCommentUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Users_UserId1",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_UserId1",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Notification");

            migrationBuilder.CreateTable(
                name: "DictionaryComments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    LikeCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DictionaryEntryId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.ComputedColumn),
                    UserId = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Content = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsEdited = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ParentCommentId = table.Column<Guid>(type: "char(36)", nullable: true, collation: "ascii_general_ci")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DictionaryComments_DictionaryComments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "DictionaryComments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DictionaryComments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DictionaryComments_entries_DictionaryEntryId",
                        column: x => x.DictionaryEntryId,
                        principalTable: "entries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryComments_DictionaryEntryId",
                table: "DictionaryComments",
                column: "DictionaryEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryComments_ParentCommentId",
                table: "DictionaryComments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryComments_UserId",
                table: "DictionaryComments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryComments");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId1",
                table: "Notification",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_UserId1",
                table: "Notification",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Users_UserId1",
                table: "Notification",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
