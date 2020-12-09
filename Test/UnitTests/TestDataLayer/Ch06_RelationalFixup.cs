// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_RelationalFixup
    {
        private readonly ITestOutputHelper _output;

        public Ch06_RelationalFixup(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestReadBookWithInclude()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            bookId = context.Books.Single(x => x.Reviews.Any()).BookId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var bookWithReviews  = context.Books
                .Include(x => x.Reviews)
                .Single(x => x.BookId == bookId);

            //VERIFY
            bookWithReviews.Reviews.Count.ShouldEqual(2);
        }

        [Fact]
        public void TestReadBookWithSecondRead()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            bookId = context.Books.Single(x => x.Reviews.Any()).BookId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var bookWithReviews = context.Books
                .Single(x => x.BookId == bookId);
            bookWithReviews.Reviews.ShouldBeNull();
            var reviews = context.Set<Review>()
                .Where(x => x.BookId == bookId)
                .ToList();

            //VERIFY
            bookWithReviews.Reviews.Count.ShouldEqual(2);
        }

        [Fact]
        public void TestBookAuthorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book1 = context.Books
                .Include(r => r.AuthorsLink)
                .ThenInclude(r => r.Author)
                .First();
            var book2 = context.Books
                .Include(r => r.AuthorsLink)
                .Skip(1).First();

            //VERIFY
            book1.AuthorsLink.First().Author.ShouldNotBeNull();
            book2.AuthorsLink.First().Author.ShouldNotBeNull();
            book1.AuthorsLink.First().Author.ShouldEqual(book2.AuthorsLink.First().Author);
        }

        [Fact]
        public void TestBookAuthorsAsNoTrackingOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book1 = context.Books
                .Include(r => r.AuthorsLink)
                .ThenInclude(r => r.Author)
                .First();
            var book2 = context.Books
                .AsNoTracking()
                .Include(r => r.AuthorsLink)
                .Skip(1).First();

            //VERIFY
            book1.AuthorsLink.First().Author.ShouldNotBeNull();
            book2.AuthorsLink.First().Author.ShouldBeNull();
        }

        [Fact]
        public void TestReadEachSeparately()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            bookId = context.Books.Single(x => x.Reviews.Any()).BookId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books.ToList();
            var reviews = context.Set<Review>().ToList();
            var authorsLinks = context.Set<BookAuthor>().ToList();
            var authors = context.Authors.ToList();

            //VERIFY
            book.Last().Reviews.Count.ShouldEqual(2);
            book.Last().AuthorsLink.Single().Author.ShouldNotBeNull();
        }
    }
}