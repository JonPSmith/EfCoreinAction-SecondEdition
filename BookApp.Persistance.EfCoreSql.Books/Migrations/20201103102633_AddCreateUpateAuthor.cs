using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApp.Persistence.EfCoreSql.Books.Migrations
{
    public partial class AddCreateUpateAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                table: "Authors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUtc",
                table: "Authors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "WhenCreatedUtc",
                table: "Authors");
        }
    }
}
