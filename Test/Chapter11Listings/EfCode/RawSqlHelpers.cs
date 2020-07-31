// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;

namespace Test.Chapter11Listings.EfCode
{
    public static class RawSqlHelpers
    {
        public const string FilterOnReviewRank = "FilterOnReviewRank";
        public const string UdfAverageVotes = "udf_AverageVotes";

        public static void AddUpdateSqlProcs(this DbContext context)
        {

            using (var transaction = context.Database.BeginTransaction())
            {
                try
                {
                    context.Database.ExecuteSqlRaw(
                        $"IF OBJECT_ID('dbo.{FilterOnReviewRank}') IS NOT NULL " +
                        $"DROP PROC dbo.{FilterOnReviewRank}");

                    context.Database.ExecuteSqlRaw(
                        $"CREATE PROC dbo.{FilterOnReviewRank}" +
                        @"(  @RankFilter int )
AS

SELECT * FROM dbo.Books b 
WHERE (SELECT AVG(CAST([NumStars] AS float)) FROM dbo.Review AS r WHERE b.BookId = r.BookId) >= @RankFilter
");



                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    //I leave this in because you normally would catch the exception and return an error.
                    throw;

                }
            }
        }

        public static bool EnsureSqlProcsSet(this DbContext context)
        {
            var connection = context.Database.GetDbConnection().ConnectionString;
            return connection.ExecuteRowCount("sysobjects", $"WHERE type='P' AND name='{FilterOnReviewRank}'") == 1
                   && connection.ExecuteRowCount("sys.objects",
                       $"WHERE object_id = OBJECT_ID(N'[dbo].[{UdfAverageVotes}]')" +
                       " AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' )") == 1;
        }
    }
}