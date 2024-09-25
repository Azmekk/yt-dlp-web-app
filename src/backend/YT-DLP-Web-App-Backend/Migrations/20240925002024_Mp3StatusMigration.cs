using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace YT_DLP_Web_App_Backend.Migrations
{
    /// <inheritdoc />
    public partial class Mp3StatusMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Mp3Status",
                table: "Videos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mp3Status",
                table: "Videos");
        }
    }
}
