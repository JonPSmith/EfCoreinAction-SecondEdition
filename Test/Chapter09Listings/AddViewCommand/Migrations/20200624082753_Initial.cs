using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Test.Chapter09Listings.AddViewCommand.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MyString = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MyInt = table.Column<int>(type: "int", nullable: false),
                    MyDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entities", x => x.Id);
                });

            migrationBuilder.AddViewViaSql<MyView>("EntityFilterView", "Entities", "MyDateTime >= '2020-1-1'");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Entities");

            migrationBuilder.Sql("DROP VIEW EntityFilterView");
        }
    }
}
