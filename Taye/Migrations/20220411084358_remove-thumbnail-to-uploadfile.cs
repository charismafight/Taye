using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Taye.Migrations
{
    public partial class removethumbnailtouploadfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Thumbnail",
                table: "Archives");

            migrationBuilder.AddColumn<string>(
                name: "ThumbnailFilePath",
                table: "UploadFiles",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ThumbnailFilePath",
                table: "UploadFiles");

            migrationBuilder.AddColumn<string>(
                name: "Thumbnail",
                table: "Archives",
                type: "text",
                nullable: true);
        }
    }
}
