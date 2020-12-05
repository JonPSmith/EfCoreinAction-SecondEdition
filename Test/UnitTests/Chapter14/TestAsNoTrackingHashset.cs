// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestAsNoTrackingHashset
    {
        private readonly ITestOutputHelper _output;

        public TestAsNoTrackingHashset(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestQueryPerformanceWithHashSetsOk()
        {
            //SETUP
            int numBooks = 100;
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            options.TurnOffDispose();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);
            }
            //ATTEMPT
            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "Normal"))
            {
                var books = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "AsNoTracking"))
            {
                var books = context.Books
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "AsNoTrackingWithIdentityResolution"))
            {
                var books = context.Books
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "Normal2"))
            {
                var books = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "AsNoTracking2"))
            {
                var books = context.Books
                    .AsNoTracking()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }

            using (var context = new BookDbContext(options))
            using (new TimeThings(_output, "AsNoTrackingWithIdentityResolution2"))
            {
                var books = context.Books
                    .AsNoTrackingWithIdentityResolution()
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                    .ToList();
            }
            options.ManualDispose();
        }
    }
}