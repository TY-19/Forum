using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnreadElements_MessageId",
                table: "UnreadElements");

            migrationBuilder.DropIndex(
                name: "IX_UnreadElements_TopicId",
                table: "UnreadElements");

            migrationBuilder.CreateIndex(
                name: "IX_UnreadElements_MessageId",
                table: "UnreadElements",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_UnreadElements_TopicId",
                table: "UnreadElements",
                column: "TopicId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnreadElements_MessageId",
                table: "UnreadElements");

            migrationBuilder.DropIndex(
                name: "IX_UnreadElements_TopicId",
                table: "UnreadElements");

            migrationBuilder.CreateIndex(
                name: "IX_UnreadElements_MessageId",
                table: "UnreadElements",
                column: "MessageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnreadElements_TopicId",
                table: "UnreadElements",
                column: "TopicId",
                unique: true);
        }
    }
}
