using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BivvySpot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Edit_PostDifficulty_Configuration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostDifficulties_PostId_DifficultyId",
                table: "PostDifficulties");

            migrationBuilder.CreateIndex(
                name: "IX_PostDifficulties_PostId",
                table: "PostDifficulties",
                column: "PostId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PostDifficulties_PostId",
                table: "PostDifficulties");

            migrationBuilder.CreateIndex(
                name: "IX_PostDifficulties_PostId_DifficultyId",
                table: "PostDifficulties",
                columns: new[] { "PostId", "DifficultyId" },
                unique: true);
        }
    }
}
