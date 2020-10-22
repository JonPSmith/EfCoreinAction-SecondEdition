// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace BookApp.Infrastructure.Books.Seeding
{
    public class BookGenerator
    {
        private readonly BookDbContext _context;
        private ManningBookLoad _loadedBookData;
        private int NumBooksInSet => _loadedBookData.Books.Count;

        public BookGenerator(BookDbContext context)
        {
            _context = context;
        }

        public async Task WriteBooksAsync(string filePath, int totalBooksNeeded, bool makeBookTitlesDistinct, CancellationToken cancellationToken)
        {
            _loadedBookData = filePath.LoadManningBooks();

            //Find out how many in db so we can pick up where we left off
            var numBooksInDb = await _context.Books.IgnoreQueryFilters().CountAsync();

            var numWritten = 0;
            if (numBooksInDb < NumBooksInSet)
            {
                //If the data in the database doesn't contain the current json set then wipe and add json books

                await _context.Database.EnsureDeletedAsync();
                await _context.Database.MigrateAsync();
                _context.AddRange(_loadedBookData.Books);
                numBooksInDb = await _context.SaveChangesAsync();
                numWritten = numBooksInDb;
            }

            var numToWrite = totalBooksNeeded - numBooksInDb;
            while (numWritten < numToWrite)
            {
                //This adds books in batches of the json Books (or shorter if near to the end)

                if (cancellationToken.IsCancellationRequested)
                    return;

                _context.ChangeTracker.Clear();

                var authorsDict = _context.Authors.ToDictionary(x => x.Name);
                var tagsDict = _context.Tags.ToDictionary(x => x.TagId);

                var batchToAdd = Math.Min(NumBooksInSet, numToWrite - numWritten);
                var batch = GenerateBooks(batchToAdd, numBooksInDb, makeBookTitlesDistinct, authorsDict, tagsDict).ToList();
                _context.AddRange(batch);
                await _context.SaveChangesAsync();
                numWritten += batch.Count;
                numBooksInDb += batch.Count;
            }
        }

        private IEnumerable<Book> GenerateBooks(int batchToAdd, int numBooksInDb, bool makeBookTitlesDistinct, 
            Dictionary<string, Author> authorDict, Dictionary<string, Tag> tagsDict)
        {
            for (int i = numBooksInDb; i < numBooksInDb + batchToAdd; i++)
            {
                var sectionNum = (int)Math.Truncate(i / (double)NumBooksInSet);

                var jsonBook = _loadedBookData.Books[i % NumBooksInSet];
                var authors = jsonBook.AuthorsLink.Select(x => x.Author.Name)
                    .Select(x => authorDict[x])
                    .ToList();
                var tags = jsonBook.Tags.Select(x => tagsDict[x.TagId])
                    .ToList();

                var book = Book.CreateBook(makeBookTitlesDistinct ?  $"{jsonBook.Title} (copy {sectionNum})" : jsonBook.Title,
                    jsonBook.PublishedOn.AddDays(sectionNum),
                    jsonBook.EstimatedDate,
                    "Manning",
                    (i + 1),
                    jsonBook.ImageUrl,
                    authors,
                    tags).Result;

                for (int j = 0; j < i % 12; j++)
                {
                    book.AddReview((Math.Abs(3 - j) % 4) + 2, null, j.ToString());
                }
                if (i % 7 == 0)
                {
                    book.AddPromotion(book.ActualPrice * 0.5m, "today only - 50% off! ");
                }

                yield return book;
            }
        }
    }
}