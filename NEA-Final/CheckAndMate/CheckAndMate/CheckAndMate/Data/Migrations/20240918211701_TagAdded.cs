using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CheckAndMate.Migrations
{
    /// <inheritdoc />
    public partial class TagAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tag",
                table: "AspNetUsers");
        }
    }
}
