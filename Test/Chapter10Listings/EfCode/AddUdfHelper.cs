// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;

namespace Test.Chapter10Listings.EfCode
{
    public static class AddUdfHelper
    {
        public const string UdfAverageVotes =
            nameof(MyUdfMethods.AverageVotes); //#A

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
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //I left this in because you noramlly would catch the expection and return an error.
                    throw;
                }
            }
        }
    }
}
