// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BookApp.Infrastructure.LoggingServices;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BookApp.ServiceLayer.DapperSql.Books.DapperCode
{
    public static class DapperQueries
    {
        private class LogDapperCommand : IDisposable
        {
            private readonly string _command;
            private readonly ILogger _myLogger;
            private readonly Stopwatch stopwatch = new Stopwatch();

            public LogDapperCommand(string command, BookDbContext context)
            {
                _command = command;
                _myLogger = context.GetService<ILoggerFactory>().CreateLogger(nameof(DapperQueries));
                stopwatch.Start();
            }

            public void Dispose()
            {
                stopwatch.Stop();
                _myLogger.LogInformation(new EventId(1, LogParts.DapperEventName), 
                    $"Dapper Query. Execute time = {stopwatch.ElapsedMilliseconds} ms.\n"+ _command);
            }
        }

        public static async Task<IEnumerable<BookListDto>> //#A
            DapperBookListQueryAsync(this BookDbContext context, //#B
                ISortFilterPage options) //#C
        {
            var command = BuildQueryString(options, false); //#D
            using(new LogDapperCommand(command, context)) //#E
            {
                return await context.Database.GetDbConnection() //#F
                    .QueryAsync<BookListDto>(command, new    //#G
                    {                                   //#G
                        pageSize = options.PageSize,    //#G
                        skipRows = options.PageSize     //#G
                            * (options.PageNum - 1),    //#G
                        filterVal = options.FilterValue //#G
                    });
            }
        }

        private static string BuildQueryString              //#H
            (ISortFilterPage options, bool justCount) //#H
        {
            var selectOptTop = FormSelectPart(options, justCount); //#I
            var filter = FormFilter(options); //#J
            if (justCount)                    //#K
                return selectOptTop + filter; //#K

            var sort = FormSort(options); //#L
            var optOffset = FormOffsetEnd(options); //#M

            return selectOptTop + filter   //#N
                + sort + optOffset + "\n"; //#N

        }
        /*****************************************************************
        #A A Dapper query returns an IEnumerable<T> result. By default, it will have read all the rows in one go, but you can change Dapper's buffered options
        #B I pass in the application's DbContext, as I am assuming most of the database accesses will be done via EF Core
        #C The options contain the settings of the sort, filter, page controls set by the user
        #D I call the method to build the correct query string based on the user options
        #E This is just some code to capture the SQL command and how long it took to execute and log it
        #F Here I get the type of connection that Dapper needs from the application's DbContext
        #G The Dapper query takes the SQL command string and an anonymous class with the variable data
        #H This is the method that combines the various parts of the SQL query. It takes in the sort, filter, page options and a boolean if the query is just counting the number of rows
        #I This forms the Select part: if is just for counting its "SELECT COUNT(*) FROM [Books] AS b", otherwise its all the various coolumns, calculated values and so on
        #J Now I build the filter, starting with "WHERE ([b].[SoftDeleted] = 0)" and filling in the rest depending on the options
        #K If its just a count we leave return this as the sort is not needed, and I don't want the offset
        #L This adds a sort of the form "ORDER BY [b].[PublishedOn] DESC" or similar
        #M For paging I need to add a OFFSET value
        #N Finally I return the compelete SQL command
         * ***************************************************************/


        public static async Task<int> DapperBookListCountAsync(this BookDbContext context, SortFilterPageOptions options)
        {
            var command = BuildQueryString(options, true);
            using (new LogDapperCommand(command, context))
            {
                return await context.Database.GetDbConnection()
                    .ExecuteScalarAsync<int>(command, new
                    {
                        filterVal = options.FilterValue
                    });
            }
        }

        private static string FormFilter(ISortFilterPage options)
        {
            const string start = "WHERE ([b].[SoftDeleted] = 0) ";
            switch (options.FilterBy)
            {
                case BooksFilterBy.NoFilter:
                    return start;
                case BooksFilterBy.ByVotes:
                    return start + @"AND ((
    SELECT AVG(CAST([y0].[NumStars] AS float))
    FROM [Review] AS [y0]
    WHERE [b].[BookId] = [y0].[BookId]
) > @filterVal)";
                case BooksFilterBy.ByTags:
                    return start + @"AND 
(@filterVal IN (SELECT [t].[TagId] FROM BookTag AS t 
WHERE [t].[BookId] = [b].[BookId])) ";
                case BooksFilterBy.ByPublicationYear:
                    return start +
@"AND (DATEPART(year, [b].[PublishedOn]) = @filterVal) 
AND ([b].[PublishedOn] <= GETUTCDATE()) ";
            }
            throw new NotImplementedException();
        }

        private static string FormSort(ISortFilterPage options)
        {
            const string start = "ORDER BY ";
            switch (options.OrderByOptions)
            {
                case OrderByOptions.SimpleOrder:
                    return start + "[b].[BookId] DESC ";
                case OrderByOptions.ByVotes:
                    return start + "[ReviewsAverageVotes] DESC ";
                case OrderByOptions.ByPublicationDate:
                    return start + "[b].[PublishedOn] DESC ";
                case OrderByOptions.ByPriceLowestFirst:
                    return start + "[ActualPrice] ";
                case OrderByOptions.ByPriceHighestFirst:
                    return start + "[ActualPrice] DESC ";
            }
            throw new NotImplementedException();
        }

        private static string FormOffsetEnd(ISortFilterPage options)
        {
            return options.PageNum <= 1
                ? ""
                : " OFFSET @skipRows ROWS FETCH NEXT @pageSize ROWS ONLY";
        }

        private static string FormSelectPart(ISortFilterPage options, bool justCount)
        {
            if (justCount)
                return "SELECT COUNT(*) FROM [Books] AS [b] ";

            var selectOpt =  options.PageNum <= 1
                ? "SELECT TOP(@pageSize) "
                : "SELECT ";

            return selectOpt +
@"[b].[BookId], [b].[Title], [b].[OrgPrice], [b].[ActualPrice],
[b].[PublishedOn],
[b].[PromotionalText] AS [PromotionPromotionalText], 
[dbo].AuthorsStringUdf([b].[BookId]) AS [AuthorsOrdered], 
[dbo].TagsStringUdf([b].[BookId]) AS [TagsString],
( SELECT COUNT(*) FROM [Review] AS [r] WHERE [b].[BookId] = [r].[BookId] ) AS [ReviewsCount], 
( SELECT AVG(CAST([y].[NumStars] AS float)) FROM [Review] AS [y] WHERE [b].[BookId] = [y].[BookId] ) AS [ReviewsAverageVotes] 
FROM [Books] AS [b]
";
        }
    }
}