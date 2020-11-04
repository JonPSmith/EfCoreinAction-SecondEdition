using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApp.Persistence.EfCoreSql.Books.Migrations
{
    public partial class IndexesOnLastUpdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Review_LastUpdatedUtc",
                table: "Review",
                column: "LastUpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Books_LastUpdatedUtc",
                table: "Books",
                column: "LastUpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_BookAuthor_LastUpdatedUtc",
                table: "BookAuthor",
                column: "LastUpdatedUtc");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_LastUpdatedUtc",
                table: "Authors",
                column: "LastUpdatedUtc");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_LastUpdatedUtc",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Books_LastUpdatedUtc",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_BookAuthor_LastUpdatedUtc",
                table: "BookAuthor");

            migrationBuilder.DropIndex(
                name: "IX_Authors_LastUpdatedUtc",
                table: "Authors");
        }
    }
}
