using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BivvySpot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Add_Properties_to_PostPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChecksumSha256",
                table: "PostPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "PostPhotos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChecksumSha256",
                table: "PostPhotos");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "PostPhotos");
        }
    }
}
