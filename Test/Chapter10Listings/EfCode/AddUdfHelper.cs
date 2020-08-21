// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public static class AddUdfHelper
    {
        public const string UdfAverageVotes =
            nameof(MyUdfMethods.AverageVotes); //#A

        public const string UdfTableFunctionName = nameof(Chapter10EfCoreContext.GetBookTitleAndReviewsFiltered);

        public static void AddUdfToDatabase(this DbContext context)
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        $"IF OBJECT_ID('dbo.{UdfAverageVotes}', N'FN') IS NOT NULL " +
                        $"DROP FUNCTION dbo.{UdfAverageVotes}");

                    context.Database.ExecuteSqlRaw( //#B
                        $"CREATE FUNCTION {UdfAverageVotes} (@bookId int)" + //#C
                        @"  RETURNS float
                          AS
                          BEGIN
                          DECLARE @result AS float
                          SELECT @result = AVG(CAST([NumStars] AS float)) 
                               FROM dbo.Review AS r
                               WHERE @bookId = r.BookId
                          RETURN @result
                          END");
                    /*****************************************************
                    #A I capture the name of the static method that represents my UDF and use it as the name of my UDF I add to the database
                    #B I use EF Core's ExecuteSqlCommand method to add the UDF into the database
                    #C The SQL code that follows adds a UDF to a SQL server database
                     * **************************************************/

                    context.Database.ExecuteSqlRaw(
                        $"IF OBJECT_ID('dbo.{UdfTableFunctionName}', N'FN') IS NOT NULL " +
                        $"DROP FUNCTION dbo.{UdfTableFunctionName}");

                    context.Database.ExecuteSqlRaw( 
                        $"CREATE FUNCTION {UdfTableFunctionName} (@minReviews int)" +
                        @"RETURNS @result TABLE
                          (
                            Title nvarchar(max) null,
                            ReviewsCount int not null,
                            AverageVotes float null
                          )
                          AS
                          BEGIN
						  INSERT into @result 
                          SELECT b.Title, 
                                 (SELECT Count(*) 
                                    FROM Review AS r
                                    WHERE b.bookId = r.BookId) AS ReviewsCount,
                                 (SELECT AVG(CAST([NumStars] AS float))
                                    FROM Review AS r
                                    WHERE b.bookId = r.BookId) AS AverageVotes
                          FROM Books AS b
                          WHERE (SELECT Count(*) 
                                    FROM Review AS r
                                    WHERE b.bookId = r.BookId) >= @minReviews

                          RETURN 
                          END");
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //I left this in because you normally would catch the exception and return an error.
                    throw;
                }
            }
        }
    }
}
