// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.SupportTypes;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookApp.Infrastructure.Books.EventHandlers.CheckFixCode
{
    public class CheckFixCacheValuesService : ICheckFixCacheValuesService
    {
        private readonly BookDbContext _context;
        private readonly CheckFixCacheOptions _options;
        private readonly ILogger<CheckFixCacheValuesService> _logger;

        public CheckFixCacheValuesService(BookDbContext context, 
            IOptions<CheckFixCacheOptions> options, 
            ILogger<CheckFixCacheValuesService> logger)
        {
            _context = context;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<DateTime> RunCheckAsync(DateTime fromThisDate)
        {
            var toThisDate = DateTime.UtcNow.Add(-_options.IgnoreIfWithinOffset);
            var bookIdsOfChanged = new HashSet<int>();
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Books, 
                    fromThisDate, toThisDate)
                    .Select(x => x.BookId).ToListAsync());
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<Review>(), 
                    fromThisDate, toThisDate)
                    .Select(x => x.BookId).ToListAsync());
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<BookAuthor>(), 
                    fromThisDate, toThisDate)
                    .Select(x => x.BookId).ToListAsync());
            bookIdsOfChanged.UnionWith(await FilterByToFrom(
                    _context.Set<Author>(),
                    fromThisDate, toThisDate)
                .SelectMany(x => x.BooksLink
                    .Select(y => y.BookId)).ToListAsync());

            var hadErrors = false;
            foreach (var bookId in bookIdsOfChanged)
            {
                var status = await _context.CheckSingleBookAsync(bookId, _options.FixBadCacheValues);
                if (status.HasErrors)
                {
                    foreach (var error in status.Errors)
                    {
                        _logger.Log(_options.FixBadCacheValues 
                            ? LogLevel.Warning : LogLevel.Error, 
                            error.ToString());
                    }
                    hadErrors = true;
                }
                await Task.Delay(_options.WaitBetweenEachCheck);
            }

            if (hadErrors && _options.FixBadCacheValues)
                await _context.SaveChangesAsync();

            return toThisDate;
        }

        private IQueryable<T> FilterByToFrom<T>(IQueryable<T> source, 
            DateTime fromThisDate, DateTime toThisDate)
            where T : ICreatedUpdated
        {
            return source.Where(x => x.LastUpdatedUtc >= fromThisDate
                                     && x.LastUpdatedUtc < toThisDate);
        }
    }
}