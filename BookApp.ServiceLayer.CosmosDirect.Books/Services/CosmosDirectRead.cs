// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Infrastructure.LoggingServices;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace BookApp.ServiceLayer.CosmosDirect.Books.Services
{
    public static class CosmosDirectRead
    {
        private class LogCosmosCommand : IDisposable
        {
            private readonly string _command;
            private readonly ILogger _myLogger;
            private readonly Stopwatch _stopwatch = new Stopwatch();

            public LogCosmosCommand(string command, CosmosDbContext context)
            {
                _command = command;
                _myLogger = context.GetService<ILoggerFactory>().CreateLogger(nameof(CosmosDirectRead));
                _stopwatch.Start();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                _myLogger.LogInformation(new EventId(1, LogParts.CosmosEventName),
                    $"Cosmos Query. Execute time = {_stopwatch.ElapsedMilliseconds} ms.\n" + _command);
            }
        }

        public static async Task<IEnumerable<CosmosBook>> 
            CosmosDirectQueryAsync(this CosmosDbContext context, 
                SortFilterPageOptions options, string databaseName)
        {
            var container = context.GetCosmosContainerFromDbContext(databaseName);

            var command = BuildQueryString(options, false);
            using (new LogCosmosCommand(command, context))
            {
                var resultSet = container.GetItemQueryIterator<CosmosBook>(new QueryDefinition(command));
                return await resultSet.ReadNextAsync();
            }
        }

        public static async Task<int> CosmosDirectCountAsync(this CosmosDbContext context,
            SortFilterPageOptions options, string databaseName)
        {
            var container = context.GetCosmosContainerFromDbContext(databaseName);

            var command = BuildQueryString(options, true);
            using (new LogCosmosCommand(command, context))
            {
                var resultSet = container.GetItemQueryIterator<int>(new QueryDefinition(command));
                return (await resultSet.ReadNextAsync()).First();
            }
        }

        private static string BuildQueryString              
            (SortFilterPageOptions options, bool justCount) 
        {
            var selectOptTop = FormSelectPart(options, justCount); 
            var filter = FormFilter(options); 
            if (justCount)                    
                return selectOptTop + filter; 

            var sort = FormSort(options);

            var skipRows = options.PageSize * (options.PageNum - 1);
            var paging = $" OFFSET {skipRows} LIMIT {options.PageSize}";

            return selectOptTop + filter   
                + sort + paging + "\n"; 

        }

        private static string FormFilter(SortFilterPageOptions options)
        {
            switch (options.FilterBy)
            {
                case BooksFilterBy.NoFilter:
                    return null;
                case BooksFilterBy.ByVotes:
                    return $" WHERE c.ReviewsAverageVotes > {options.FilterValue} ";
                case BooksFilterBy.ByTags:
                    return $" WHERE CONTAINS(c.TagsString, '| {options.FilterValue} |') ";
                    //return $" JOIN f in c.Tags WHERE f.TagId = '{options.FilterValue}'";
                case BooksFilterBy.ByPublicationYear:
                    return options.FilterValue == DisplayConstants.AllBooksNotPublishedString 
                        ? $" WHERE c.PublishedOn > '{DateTime.UtcNow:yyyy-MM-dd}' " 
                        : $" WHERE c.YearPublished = {options.FilterValue} AND c.PublishedOn < '{DateTime.UtcNow:yyyy-MM-dd}' ";
            }
            throw new NotImplementedException();
        }

        private static string FormSort(SortFilterPageOptions options)
        {
            const string start = "ORDER BY ";
            switch (options.OrderByOptions)
            {
                case OrderByOptions.SimpleOrder:
                    return start + " c.BookId DESC";
                case OrderByOptions.ByVotes:
                    return start + " c.ReviewsAverageVotes DESC";
                case OrderByOptions.ByPublicationDate:
                    return start + " c.PublishedOn DESC";
                case OrderByOptions.ByPriceLowestFirst:
                    return start + " c.ActualPrice ";
                case OrderByOptions.ByPriceHighestFirst:
                    return start + " c.ActualPrice DESC ";
            }
            throw new NotImplementedException();
        }

        private static string FormSelectPart(SortFilterPageOptions options, bool justCount)
        {
            if (justCount)
                return "SELECT value COUNT(c) FROM c";


            return
@"SELECT c.BookId, c.Title, c.PublishedOn, c.EstimatedDate, c.YearPublished,
c.OrgPrice, c.ActualPrice, c.PromotionalText, c.ManningBookUrl,
c.AuthorsOrdered, c.ReviewsCount, c.ReviewsAverageVotes, c.Tags, c.TagsString
FROM c
";
        }

    }
}