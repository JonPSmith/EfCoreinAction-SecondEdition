// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookApp.Infrastructure.Books.Seeding
{
    public class BookGenerator : IBookGenerator
    {
        private readonly DbContextOptions<BookDbContext> _sqlOptions;
        private readonly DbContextOptions<CosmosDbContext> _cosmosOptions;
        private List<Book> _loadedBooks;

        private const int AddPromotionEvery = 7;
        private const int MaxReviewsPerBook = 12;
        private readonly Random _random = new Random(1); //Used to create random review star ratings. Seeded for same sequence

        public BookGenerator(IServiceProvider provider)
        {
            _sqlOptions = provider.GetRequiredService<DbContextOptions<BookDbContext>>();
            //If cosmos not turned on then this will be null
            _cosmosOptions = provider.GetService<DbContextOptions<CosmosDbContext>>();
        }

        private int NumBooksInSet => _loadedBooks.Count;
        private bool IncludeCosmosDb => _cosmosOptions != null;

        public async Task<TimeSpan> WriteBooksAsync(string wwwRootDir, bool wipeDatabase, int totalBooksNeeded, 
            bool makeBookTitlesDistinct, CancellationToken cancellationToken)
        {
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Find out how many in db so we can pick up where we left off
            int numBooksInDb;
            using (var context = new BookDbContext(_sqlOptions))
                numBooksInDb = await context.Books.IgnoreQueryFilters().CountAsync();

            _loadedBooks = wwwRootDir.LoadManningBooks(false).ToList();
            if (wipeDatabase || numBooksInDb < NumBooksInSet)
                using (var context = new BookDbContext(_sqlOptions))
                {
                    //If the data in the database doesn't contain the current json set then wipe and add json books

                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();

                    var books = wwwRootDir.LoadManningBooks(true).ToList();
                    books.ForEach(SetCreatedUpdated);
                    //Assumes no reviews
                    context.AddRange(books);
                    await context.SaveChangesAsync();
                    await OptionalCosmosWriteAsync(books, true);

                    numBooksInDb = await context.Books.IgnoreQueryFilters().CountAsync();
                }

            var numWritten = 0;
            var numToWrite = totalBooksNeeded - numBooksInDb;
            while (numWritten < numToWrite)
            {
                //This adds books in batches of the json Books (or shorter if near to the end)

                if (cancellationToken.IsCancellationRequested)
                    return stopWatch.Elapsed;

                var numInBatch =
                    await GenerateBatchAndWrite(makeBookTitlesDistinct, numToWrite, numWritten, numBooksInDb);
                numWritten += numInBatch;
                numBooksInDb += numInBatch;
            }

            return stopWatch.Elapsed;
        }

        private async ValueTask OptionalCosmosWriteAsync(List<Book> books, bool wipeDatabase = false)
        {
            if (!IncludeCosmosDb)
                return;

            using var cosmosContext = new CosmosDbContext(_cosmosOptions);
            if (wipeDatabase)
            {
                await cosmosContext.Database.EnsureDeletedAsync();
                await cosmosContext.Database.EnsureCreatedAsync();
            }

            var cosmosBooks = books.Select(b => new CosmosBook
            {
                BookId = b.BookId,
                Title = b.Title,
                PublishedOn = b.PublishedOn,
                EstimatedDate = b.EstimatedDate,
                YearPublished = b.PublishedOn.Year,
                OrgPrice = b.OrgPrice,
                ActualPrice = b.ActualPrice,
                PromotionalText = b.PromotionalText,
                ManningBookUrl = b.ManningBookUrl,
                AuthorsOrdered = string.Join(", ",
                    b.AuthorsLink
                        .OrderBy(q => q.Order)
                        .Select(q => q.Author.Name)),
                ReviewsCount = b.Reviews?.Count ?? 0,
                ReviewsAverageVotes = b.Reviews != null && b.Reviews.Any()
                    ? b.Reviews.Average(y => y.NumStars)
                    : 0.0,
                Tags = b.Tags
                    .Select(x => new CosmosTag(x.TagId)).ToList(),
                TagsString = $"| {string.Join(" | ", b.Tags.Select(x => x.TagId))} |"
            });
            cosmosContext.AddRange(cosmosBooks);
            await cosmosContext.SaveChangesAsync();
        }

        private async Task<int> GenerateBatchAndWrite(bool makeBookTitlesDistinct, int numToWrite,
            int numWritten, int numBooksInDb)
        {
            using var context = new BookDbContext(_sqlOptions);
            var authorsDict = context.Authors.ToDictionary(x => x.Name);
            var tagsDict = context.Tags.ToDictionary(x => x.TagId);

            var batchToAdd = Math.Min(NumBooksInSet, numToWrite - numWritten);
            var batch = GenerateBooks(batchToAdd, numBooksInDb, makeBookTitlesDistinct, authorsDict, tagsDict)
                .ToList();
            context.AddRange(batch);
            await context.SaveChangesAsync();
            await OptionalCosmosWriteAsync(batch);
            return batch.Count;
        }

        private IEnumerable<Book> GenerateBooks(int batchToAdd, int numBooksInDb, bool makeBookTitlesDistinct, 
            Dictionary<string, Author> authorDict, Dictionary<string, Tag> tagsDict)
        {
            for (int i = numBooksInDb; i < numBooksInDb + batchToAdd; i++)
            {
                var sectionNum = (int)Math.Truncate(i / (double)NumBooksInSet);

                var jsonBook = _loadedBooks[i % NumBooksInSet];
                var authors = jsonBook.AuthorsLink.Select(x => x.Author.Name)
                    .Select(x => authorDict[x])
                    .ToList();
                var tags = jsonBook.Tags.Select(x => tagsDict[x.TagId])
                    .ToList();

                var reviewNumStars = new List<byte>();
                for (int j = 0; j < i % MaxReviewsPerBook; j++)
                {
                    reviewNumStars.Add((byte)_random.Next(0, 6));
                }

                var book = new Book(makeBookTitlesDistinct ?  $"{jsonBook.Title} (copy {sectionNum})" : jsonBook.Title,
                    jsonBook.PublishedOn.AddDays(sectionNum),
                    jsonBook.EstimatedDate,
                    ManningBookLoad.PublisherString,
                    (i + 1),
                    jsonBook.ImageUrl,
                    authors,
                    tags,
                    reviewNumStars, $"User{i:7}");
                SetCreatedUpdated(book);

                if (i % AddPromotionEvery == 0)
                {
                    book.AddPromotion(book.ActualPrice * 0.5m, "today only - 50% off! ");
                }

                yield return book;
            }
        }

        private void SetCreatedUpdated(Book book)
        {
            book.LogAddUpdate(true);
            if (book.Reviews != null)
                foreach (var review in book.Reviews)
                {
                    review.LogAddUpdate(true);
                }

            foreach (var bookAuthor in book.AuthorsLink)
            {
                bookAuthor.LogAddUpdate(true);
            }
        }
    }
}