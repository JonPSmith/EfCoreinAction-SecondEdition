
SELECT TOP(2)                             //#A


   [p].[BookId], 						  //#B
   [p].[Description], 					  //#B
   [p].[ImageUrl], 						  //#B
   [p].[Price], 						  //#B
   [p].[PublishedOn], 					  //#B
   [p].[Publisher], 					  //#B
   [p].[Title]							  //#B
FROM [Books] AS [p]						  //#B
WHERE [p].[Title] = N'Quantum Networking' //#C

SET NOCOUNT ON;
UPDATE [Books]                            //#D
   SET [PublishedOn] = @p0  			  //#E

WHERE [BookId] = @p1;					  //#F
SELECT @@ROWCOUNT;						  //#G
#A This reads up to 2 entries from the Books table - we asked for a single item in out code, but this makes sure it fails if there is more than one row that fits
#B The read loads all the columns in the table
#C This is our LINQ Where method. It picks out the entry by its title
#D This is the SQL UPDATE command in this case on the Books table
#E Because EF Core's DetectChanges method found only the PublishedOn property had changed it can target that column in the table
#F EF Core used the primary key from the original book to uniquely select the row it wants to update
#G Finally it sends back the number of rows that were inserted in this transaction. SaveChanges returns this integer, but normally I ignore it.

