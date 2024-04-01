using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForumPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Forums");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Forums",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Position",
                table: "Forums",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Forums_CategoryId",
                table: "Forums",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Forums_Category_CategoryId",
                table: "Forums",
                column: "CategoryId",
                principalTable: "Category",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Forums_Category_CategoryId",
                table: "Forums");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropIndex(
                name: "IX_Forums_CategoryId",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Forums");

            migrationBuilder.DropColumn(
                name: "Position",
                table: "Forums");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Forums",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
