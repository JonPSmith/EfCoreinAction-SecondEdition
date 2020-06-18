using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.SeedExample.Migrations
{
    public partial class UpdateSeed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Name", "Address_Street" },
                values: new object[] { "NEW Jill", "Street1" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Name", "ProjectId", "Address_City", "Address_Street" },
                values: new object[] { 3, "Jack3", 2, "city2", "Street3" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "Name", "Address_Street" },
                values: new object[] { "Jill", "Jill street" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Name", "ProjectId", "Address_City", "Address_Street" },
                values: new object[] { 2, "Jack", 2, "city2", "Jack street" });
        }
    }
}
