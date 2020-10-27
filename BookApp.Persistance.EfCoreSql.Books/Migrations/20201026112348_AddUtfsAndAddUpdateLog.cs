using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApp.Persistence.EfCoreSql.Books.Migrations
{
    public partial class AddUtfsAndAddUpdateLog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                table: "Review",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUtc",
                table: "Review",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUtc",
                table: "Books",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedUtc",
                table: "BookAuthor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "WhenCreatedUtc",
                table: "BookAuthor",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Books_ActualPrice",
                table: "Books",
                column: "ActualPrice");

            migrationBuilder.Sql(@"CREATE FUNCTION AuthorsStringUdf (@bookId int)
RETURNS NVARCHAR(4000)
AS
BEGIN
-- Thanks to https://stackoverflow.com/a/194887/1434764
DECLARE @Names AS NVARCHAR(4000)
SELECT @Names = COALESCE(@Names + ', ', '') + a.Name
FROM Authors AS a, Books AS b, BookAuthor AS ba 
WHERE ba.BookId = @bookId
      AND ba.AuthorId = a.AuthorId 
	  AND ba.BookId = b.BookId
ORDER BY ba.[Order]
RETURN @Names
END");

            migrationBuilder.Sql(@"CREATE FUNCTION TagsStringUdf (@bookId int)
RETURNS NVARCHAR(4000)
AS
BEGIN
-- Thanks to https://stackoverflow.com/a/194887/1434764
DECLARE @Tags AS NVARCHAR(4000)
SELECT @Tags = COALESCE(@Tags + ' | ', '') + t.TagId
FROM BookTag AS t, Books AS b 
WHERE t.BookId = @bookId AND b.BookId =  @bookId
RETURN @Tags
END
GO");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Books_ActualPrice",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "WhenCreatedUtc",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "WhenCreatedUtc",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastUpdatedUtc",
                table: "BookAuthor");

            migrationBuilder.DropColumn(
                name: "WhenCreatedUtc",
                table: "BookAuthor");

            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.AuthorsStringUdf') IS NOT NULL
	DROP FUNCTION dbo.AuthorsStringUdf");

            migrationBuilder.Sql(@"IF OBJECT_ID('dbo.TagsStringUdf') IS NOT NULL
	DROP FUNCTION dbo.TagsStringUdf");
        }
    }
}
