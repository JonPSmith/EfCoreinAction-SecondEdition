// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.SupportTypes;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.CheckFixCode
{
    public class CheckFixCacheValuesService : ICheckFixCacheValuesService
    {
        private readonly BookDbContext _context;
        private readonly ILogger<CheckFixCacheValuesService> _logger;
        
        public CheckFixCacheValuesService(BookDbContext context,
            ILogger<CheckFixCacheValuesService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<string>> RunCheckAsync(DateTime fromThisDate, bool fixBadCacheValues, CancellationToken cancellationToken)
        {
            var bookIdsOfChanged = new HashSet<int>();
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Books, 
                    fromThisDate)
                    .Select(x => x.BookId).ToListAsync(cancellationToken));
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<Review>(), 
                    fromThisDate)
                    .Select(x => x.BookId).ToListAsync(cancellationToken));
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<BookAuthor>(), 
                    fromThisDate)
                    .Select(x => x.BookId).ToListAsync(cancellationToken));
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<Author>(),
                    fromThisDate)
                .SelectMany(x => x.BooksLink
                    .Select(y => y.BookId)).ToListAsync(cancellationToken));


            var errors = new List<string>();
            var numErrors = 0;

            List<string> FormFinalReturn(bool cancelled)
            {
                var whatDone = errors.Any()
                    ? $"{numErrors} errors " + (fixBadCacheValues && !cancelled ? "and fixed them" : "(not fixed)")
                    : "no errors";
                errors.Insert(0, $"Looked at {bookIdsOfChanged.Count} SQL books and found {whatDone}");
                if(cancelled)
                    errors.Insert(0, $"CANCELLED");

                return errors;
            }

            foreach (var bookId in bookIdsOfChanged)
            {
                if (cancellationToken.IsCancellationRequested)
                    return FormFinalReturn(true);

                var status = await _context.CheckSingleBookAsync(bookId, fixBadCacheValues, cancellationToken);
                if (status.HasErrors)
                {
                    errors.Add($"BookId: {bookId:########}");
                    var indent = "    ";
                    foreach (var error in status.Errors)
                    {
                        _logger.Log(fixBadCacheValues 
                            ? LogLevel.Warning : LogLevel.Error, 
                            error.ToString());

                        errors.Add(indent + error);
                    }

                    numErrors++;
                }
            }

            if (numErrors > 0 && fixBadCacheValues)
                await _context.SaveChangesAsync(cancellationToken);

            return FormFinalReturn(false); 
        }

        private IQueryable<T> FilterByToFrom<T>(DbSet<T> source,
            DateTime fromThisDate)
            where T : class, ICreatedUpdated
        {
            return source.IgnoreQueryFilters().Where(x => x.LastUpdatedUtc >= fromThisDate);
        }
    }
}