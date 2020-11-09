// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Infrastructure.Books.CosmosDb.Services
{
    public class BookToCosmosBookService : IBookToCosmosBookService
    {
        private readonly BookDbContext _sqlContext;
        private readonly CosmosDbContext _cosmosContext;

        public BookToCosmosBookService(BookDbContext sqlContext, CosmosDbContext cosmosContext)
        {
            _sqlContext = sqlContext ?? throw new ArgumentNullException(nameof(sqlContext));
            _cosmosContext = cosmosContext ?? throw new ArgumentNullException(nameof(cosmosContext));
        }

        public async Task AddCosmosBookAsync(Book sqlBook)
        {
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
            if (sqlBook == null) throw new ArgumentNullException(nameof(sqlBook));

            var cosmosBook = await MapBookToCosmosBookAsync(sqlBook);
            if (cosmosBook != null)
            {
                _cosmosContext.Update(cosmosBook);
                await _cosmosContext.SaveChangesAsync();
            }
            else
            {
                await DeleteCosmosBookAsync(sqlBook);
            }
        }

        public async Task DeleteCosmosBookAsync(Book sqlBook)
        {
            if (sqlBook == null) throw new ArgumentNullException(nameof(sqlBook));

            var cosmosBook = new CosmosBook {BookId = sqlBook.BookId};
            _cosmosContext.Remove(cosmosBook);
            await _cosmosContext.SaveChangesAsync();
        }


        private async Task<CosmosBook> MapBookToCosmosBookAsync(Book sqlBook)
        {
            var cosmosBook = await _sqlContext.Books.Select(p => new CosmosBook
            {
                AuthorsOrdered = string.Join(", ",
                    p.AuthorsLink
                        .OrderBy(q => q.Order)
                        .Select(q => q.Author.Name)),
                ReviewsCount = p.Reviews.Count(),
                ReviewsAverageVotes =
                    p.Reviews.Select(y =>
                        (double?) y.NumStars).Average() ?? 0,
                Tags = sqlBook.Tags
                    .Select(x => new CosmosTag(x.TagId)).ToList()
            }).SingleOrDefaultAsync(x => x.BookId == sqlBook.BookId);

            if (cosmosBook == null)
                return null;

            cosmosBook.BookId = sqlBook.BookId;
            cosmosBook.Title = sqlBook.Title;
            cosmosBook.PublishedOn = sqlBook.PublishedOn;
            cosmosBook.EstimatedDate = sqlBook.EstimatedDate;
            cosmosBook.OrgPrice = sqlBook.OrgPrice;
            cosmosBook.ActualPrice = sqlBook.ActualPrice;
            cosmosBook.PromotionalText = sqlBook.PromotionalText;
            cosmosBook.ManningBookUrl = sqlBook.ManningBookUrl;

            return cosmosBook;
        }

        
    }
}