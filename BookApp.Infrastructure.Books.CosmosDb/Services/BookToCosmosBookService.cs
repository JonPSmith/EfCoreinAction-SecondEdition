// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Infrastructure.Books.CosmosDb.Services
{
    public class BookToCosmosBookService : IBookToCosmosBookService
    {
        private readonly BookDbContext _sqlContext;
        private readonly CosmosDbContext _cosmosContext;

        private bool CosmosNotConfigured => _cosmosContext == null;

        public BookToCosmosBookService(BookDbContext sqlContext, CosmosDbContext cosmosContext)
        {
            _sqlContext = sqlContext ?? throw new ArgumentNullException(nameof(sqlContext));
            _cosmosContext = cosmosContext;
        }

        public async Task AddCosmosBookAsync(Book sqlBook)
        {
            if (CosmosNotConfigured)
                return;

            if (sqlBook == null) throw new ArgumentNullException(nameof(sqlBook));

            var cosmosBook = await MapBookToCosmosBookAsync(sqlBook);
            if (cosmosBook != null)
            {
                _cosmosContext.Add(cosmosBook);
                await _cosmosContext.SaveChangesAsync();
            }
            else
            {
                await DeleteCosmosBookAsync(sqlBook);
            }
        }

        public async Task UpdateCosmosBookAsync(Book sqlBook)
        {
            if (CosmosNotConfigured)
                return;

            if (sqlBook == null) throw new ArgumentNullException(nameof(sqlBook));

            var cosmosBook = await MapBookToCosmosBookAsync(sqlBook);
            if (cosmosBook != null)
            {
                _cosmosContext.Update(cosmosBook);
                try
                {
                    await _cosmosContext.SaveChangesAsync();
                }
                catch (CosmosException e)
                {
                    if (e.StatusCode != HttpStatusCode.NotFound)
                        throw;
                    //If couldn't update the entry it creates an entry 
                    _cosmosContext.Entry(cosmosBook).State = EntityState.Detached;
                    await AddCosmosBookAsync(sqlBook);
                }
            }
            else
            {
                await DeleteCosmosBookAsync(sqlBook);
            }
        }

        public async Task DeleteCosmosBookAsync(Book sqlBook)
        {
            if (CosmosNotConfigured)
                return;

            if (sqlBook == null) throw new ArgumentNullException(nameof(sqlBook));

            var cosmosBook = new CosmosBook {BookId = sqlBook.BookId};
            _cosmosContext.Remove(cosmosBook);

            try
            {
                await _cosmosContext.SaveChangesAsync();
            }
            catch (CosmosException e)
            {
                if (e.StatusCode != HttpStatusCode.NotFound)
                    //We ignore a delete that didn't work
                    throw;
            }
        }

        public async Task UpdateManyCosmosBookAsync(List<int> bookIds)
        {
            var updatedCosmosBooks = await MapManyBooksToCosmosBookAsync(bookIds);
            foreach (var cosmosBook in updatedCosmosBooks)
            {
                _cosmosContext.Update(cosmosBook);
            }
            await _cosmosContext.SaveChangesAsync();
        }


        private async Task<CosmosBook> MapBookToCosmosBookAsync(Book sqlBook)
        {
            return await MapBookToCosmosBook(_sqlContext.Books
                .Where(x => x.BookId == sqlBook.BookId))
                .SingleOrDefaultAsync();
        }

        private async Task<List<CosmosBook>> MapManyBooksToCosmosBookAsync(List<int> bookIds)
        {
            return await MapBookToCosmosBook(_sqlContext.Books
                    .Where(x => bookIds.Contains(x.BookId)))
                .ToListAsync();
        }

        private IQueryable<CosmosBook> MapBookToCosmosBook(IQueryable<Book> books)
        {
            return books
                .Select(p => new CosmosBook
                {
                    BookId = p.BookId,
                    Title = p.Title,
                    PublishedOn = p.PublishedOn,
                    EstimatedDate = p.EstimatedDate,
                    YearPublished = p.PublishedOn.Year,
                    OrgPrice = p.OrgPrice,
                    ActualPrice = p.ActualPrice,
                    PromotionalText = p.PromotionalText,
                    ManningBookUrl = p.ManningBookUrl,

                    AuthorsOrdered = string.Join(", ",
                        p.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)),
                    ReviewsCount = p.Reviews.Count(),
                    ReviewsAverageVotes =
                        p.Reviews.Select(y =>
                            (double?)y.NumStars).Average(),
                    Tags = p.Tags
                        .Select(x => new CosmosTag(x.TagId)).ToList()
                });
        }


    }
}