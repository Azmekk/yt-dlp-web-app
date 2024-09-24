using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YT_DLP_Web_App_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Mp3FileNameMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Mp3FileName",
                table: "Videos",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mp3FileName",
                table: "Videos");
        }
    }
}
