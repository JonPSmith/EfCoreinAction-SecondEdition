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

namespace BookApp.Infrastructure.Books.EventHandlers.CheckFixCode
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

        public async Task RunCheckAsync(DateTime fromThisDate, bool fixBadCacheValues, CancellationToken cancellationToken)
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

            var hadErrors = false;
            foreach (var bookId in bookIdsOfChanged)
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                var status = await _context.CheckSingleBookAsync(bookId, fixBadCacheValues, cancellationToken);
                if (status.HasErrors)
                {
                    foreach (var error in status.Errors)
                    {
                        _logger.Log(fixBadCacheValues 
                            ? LogLevel.Warning : LogLevel.Error, 
                            error.ToString());
                    }
                    hadErrors = true;
                }
            }

            if (hadErrors && fixBadCacheValues)
                await _context.SaveChangesAsync(cancellationToken);
        }

        private IQueryable<T> FilterByToFrom<T>(IQueryable<T> source,
            DateTime fromThisDate)
            where T : ICreatedUpdated
        {
            return source.Where(x => x.LastUpdatedUtc >= fromThisDate);
        }
    }
}