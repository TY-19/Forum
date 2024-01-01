using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePermissionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForumEntityPermission");

            migrationBuilder.AddColumn<int>(
                name: "ForumId",
                table: "Permissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_ForumId",
                table: "Permissions",
                column: "ForumId");

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Forums_ForumId",
                table: "Permissions",
                column: "ForumId",
                principalTable: "Forums",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Forums_ForumId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_ForumId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ForumId",
                table: "Permissions");

            migrationBuilder.CreateTable(
                name: "ForumEntityPermission",
                columns: table => new
                {
                    ForumsId = table.Column<int>(type: "int", nullable: false),
                    PermissionsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumEntityPermission", x => new { x.ForumsId, x.PermissionsId });
                    table.ForeignKey(
                        name: "FK_ForumEntityPermission_Forums_ForumsId",
                        column: x => x.ForumsId,
                        principalTable: "Forums",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForumEntityPermission_Permissions_PermissionsId",
                        column: x => x.PermissionsId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ForumEntityPermission_PermissionsId",
                table: "ForumEntityPermission",
                column: "PermissionsId");
        }
    }
}
