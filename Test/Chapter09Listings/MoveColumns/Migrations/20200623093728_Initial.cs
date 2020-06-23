using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.MoveColumns.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "City", "Name", "Street" },
                values: new object[] { 123, "Jack City", "Jack", "Jake Street" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "City", "Name", "Street" },
                values: new object[] { 456, "Jill City", "Jill", "Jill Street" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
