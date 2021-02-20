using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApp.Persistence.EfCoreSql.Orders.Migrations
{
    public partial class IndexForUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Orders_UserId",
                table: "Orders");
        }
    }
}
