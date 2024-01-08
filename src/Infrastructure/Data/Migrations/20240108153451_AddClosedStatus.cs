using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddClosedStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Topics",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsClosed",
                table: "Forums",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Topics");

            migrationBuilder.DropColumn(
                name: "IsClosed",
                table: "Forums");
        }
    }
}
