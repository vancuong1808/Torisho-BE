using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Torisho.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetLevelToRoom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxParticipants",
                table: "Rooms",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<string>(
                name: "TargetLevel",
                table: "Rooms",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_Matching",
                table: "Rooms",
                columns: new[] { "Status", "TargetLevel", "RoomType" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Rooms_Matching",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "MaxParticipants",
                table: "Rooms");

            migrationBuilder.DropColumn(
                name: "TargetLevel",
                table: "Rooms");
        }
    }
}
