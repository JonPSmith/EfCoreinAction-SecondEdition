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

        public async Task AddCosmosBookAsync(int bookId)
        {
            if (CosmosNotConfigured)
                return;

            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            var cosmosBook = await MapBookToCosmosBookAsync(bookId);
            if (cosmosBook != null)
            {
                _cosmosContext.Add(cosmosBook);
                await CosmosSaveChangesWithChecksAsync(WhatDoing.Adding, bookId);
            }
            else
            {
                await DeleteCosmosBookAsync(bookId);
            }
        }

        public async Task UpdateCosmosBookAsync(int bookId)
        {
            if (CosmosNotConfigured)
                return;

            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            var cosmosBook = await MapBookToCosmosBookAsync(bookId);
            if (cosmosBook != null)
            {
                _cosmosContext.Update(cosmosBook);
                await CosmosSaveChangesWithChecksAsync(WhatDoing.Updating, bookId);
            }
            else
            {
                await DeleteCosmosBookAsync(bookId);
            }
        }

        public async Task DeleteCosmosBookAsync(int bookId)
        {
            if (CosmosNotConfigured)
                return;

            if (bookId == null) throw new ArgumentNullException(nameof(bookId));

            var cosmosBook = new CosmosBook {BookId = (int)bookId};
            _cosmosContext.Remove(cosmosBook);
            await CosmosSaveChangesWithChecksAsync(WhatDoing.Deleting, bookId);
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

        private enum WhatDoing {Adding, Updating, Deleting}

        private async Task CosmosSaveChangesWithChecksAsync(WhatDoing whatDoing, int bookId)
        {
            try
            {
                await _cosmosContext.SaveChangesAsync();
            }
            catch (DbUpdateException e)
            {
                var cosmosException = e.InnerException as CosmosException;
                if (cosmosException?.StatusCode == HttpStatusCode.Conflict && whatDoing == WhatDoing.Adding)
                {
                    var updateVersion = _cosmosContext.Find<CosmosBook>(bookId);
                    _cosmosContext.Entry(updateVersion).State = EntityState.Detached;
                    await UpdateCosmosBookAsync(bookId);
                    await CosmosSaveChangesWithChecksAsync(WhatDoing.Updating, bookId);
                }
                else
                {
                    throw;
                }
            }
            catch (CosmosException e)
            {

                if (e.StatusCode == HttpStatusCode.NotFound && whatDoing == WhatDoing.Updating)
                {
                    var updateVersion = _cosmosContext.Find<CosmosBook>(bookId);
                    _cosmosContext.Entry(updateVersion).State = EntityState.Detached;
                    await AddCosmosBookAsync(bookId);
                    await CosmosSaveChangesWithChecksAsync(WhatDoing.Adding, bookId);
                }
                else if (e.StatusCode == HttpStatusCode.NotFound && whatDoing == WhatDoing.Deleting)
                {
                    //Do nothing
                }
                else
                {
                    throw;
                }
            }
        }


        private async Task<CosmosBook> MapBookToCosmosBookAsync(int? bookId)
        {
            return await MapBookToCosmosBook(_sqlContext.Books
                    .IgnoreQueryFilters()
                    .Where(x => x.BookId == bookId))
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