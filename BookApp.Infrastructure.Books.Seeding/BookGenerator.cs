// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;

namespace BookApp.Infrastructure.Books.Seeding
{
    public class BookGenerator : IBookGenerator
    {
        private readonly DbContextOptions<BookDbContext> _dbOptions;
        private List<Book> _loadedBooks;

        private const int addPromotionEvery = 7;
        private const int maxReviewsPerBook = 12;
        private Random _random = new Random(1); //Used to create random review star ratings. Seeded for same sequence

        public BookGenerator(DbContextOptions<BookDbContext> dbOptions)
        {
            _dbOptions = dbOptions;
        }

        private int NumBooksInSet => _loadedBooks.Count;


        public async Task WriteBooksAsync(string wwwRootDir, bool wipeDatabase, int totalBooksNeeded, bool makeBookTitlesDistinct, CancellationToken cancellationToken)
        {
            //Find out how many in db so we can pick up where we left off
            int numBooksInDb = 0;
            using (var context = new BookDbContext(_dbOptions))
                numBooksInDb = await context.Books.IgnoreQueryFilters().CountAsync();

            _loadedBooks = wwwRootDir.LoadManningBooks(false).ToList();
            if (wipeDatabase || numBooksInDb < NumBooksInSet)
                using (var context = new BookDbContext(_dbOptions))
                {
                    //If the data in the database doesn't contain the current json set then wipe and add json books

                    await context.Database.EnsureDeletedAsync();
                    await context.Database.MigrateAsync();
                    //Assumes no reviews
                    context.AddRange(wwwRootDir.LoadManningBooks(true));
                    await context.SaveChangesAsync();
                    numBooksInDb = await context.Books.IgnoreQueryFilters().CountAsync();
                }

            var numWritten = 0;
            var numToWrite = totalBooksNeeded - numBooksInDb;
            while (numWritten < numToWrite)
            {
                //This adds books in batches of the json Books (or shorter if near to the end)

                if (cancellationToken.IsCancellationRequested)
                    return;

                var numInBatch =
                    await GenerateBatchAndWrite(makeBookTitlesDistinct, numToWrite, numWritten, numBooksInDb);
                numWritten += numInBatch;
                numBooksInDb += numInBatch;
            }
        }

        private async Task<int> GenerateBatchAndWrite(bool makeBookTitlesDistinct, int numToWrite,
            int numWritten, int numBooksInDb)
        {
            using var context = new BookDbContext(_dbOptions);
            var authorsDict = context.Authors.ToDictionary(x => x.Name);
            var tagsDict = context.Tags.ToDictionary(x => x.TagId);

            var batchToAdd = Math.Min(NumBooksInSet, numToWrite - numWritten);
            var batch = GenerateBooks(batchToAdd, numBooksInDb, makeBookTitlesDistinct, authorsDict, tagsDict)
                .ToList();
            context.AddRange(batch);
            await context.SaveChangesAsync();
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
                for (int j = 0; j < i % maxReviewsPerBook; j++)
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

                if (i % addPromotionEvery == 0)
                {
                    book.AddPromotion(book.ActualPrice * 0.5m, "today only - 50% off! ");
                }

                yield return book;
            }
        }
    }
}