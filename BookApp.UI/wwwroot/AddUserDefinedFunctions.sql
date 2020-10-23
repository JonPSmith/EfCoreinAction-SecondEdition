-- SQL script file to add SQL code to improve performance
-- I have built this as an Idempotent Script, that is, it can be applied even if there isn't a change and it will ensure the database is up to date

IF OBJECT_ID('dbo.AuthorsStringUdf') IS NOT NULL
	DROP FUNCTION dbo.AuthorsStringUdf
GO

CREATE FUNCTION AuthorsStringUdf (@bookId int)
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
END
GO

IF OBJECT_ID('dbo.TagsStringUdf') IS NOT NULL
	DROP FUNCTION dbo.TagsStringUdf
GO

CREATE FUNCTION TagsStringUdf (@bookId int)
RETURNS NVARCHAR(4000)
AS
BEGIN
-- Thanks to https://stackoverflow.com/a/194887/1434764
DECLARE @Tags AS NVARCHAR(4000)
SELECT @Tags = COALESCE(@Tags + ', ', '') + t.TagId
FROM BookTag AS t, Books AS b 
WHERE t.BookId = @bookId AND b.BookId =  @bookId
RETURN @Tags
END
GO