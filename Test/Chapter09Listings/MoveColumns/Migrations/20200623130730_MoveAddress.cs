using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.MoveColumns.Migrations
{
    public partial class MoveAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {


            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Addresses",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Addresses", x => x.AddressId);
                });

            migrationBuilder.Sql("ALTER TABLE [dbo].[Addresses] ADD [UserId] [int] NOT NULL");

            migrationBuilder.Sql(@"INSERT INTO [dbo].[Addresses] ([UserId],[Street],[City])
SELECT [UserId],[Street],[City] FROM [dbo].[Users]");

            migrationBuilder.Sql(@"UPDATE [dbo].[Users] 
SET [AddressId] = (SELECT [AddressId] 
    FROM [dbo].[Addresses] 
    WHERE [dbo].[Addresses].[UserId] = [dbo].[Users].[Userid])");

            migrationBuilder.Sql("ALTER TABLE [dbo].[Addresses] DROP COLUMN [UserId]");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AddressId",
                table: "Users",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users",
                column: "AddressId",
                principalTable: "Addresses",
                principalColumn: "AddressId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
                name: "City",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Addresses_AddressId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Addresses");

            migrationBuilder.DropIndex(
                name: "IX_Users_AddressId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
