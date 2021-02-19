using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApp.Persistence.EfCoreSql.Books.Migrations
{
    public partial class SoftDeleteIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Books_SoftDeleted",
                table: "Books",
                column: "SoftDeleted");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_SoftDeleted",
                table: "Books");
        }
    }
}
