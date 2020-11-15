// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestChangeTrackerClear
    {
        private readonly ITestOutputHelper _output;

        public TestChangeTrackerClear(ITestOutputHelper output)
        {
            _output = output;
        }





        [Fact]
        public void TestCreateBookAndReadBackOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var book = AddBookWithTwoReviewsToDatabase(context);

            //ATTEMPT
            var readBook = context.Books.Single(x => x.BookId == book.BookId);

            //VERIFY
            _output.WriteLine($"Book has {readBook.Reviews.Count} reviews");
        }








        [Fact]
        public void TestCreateBookAndReadBackWithChangeTrackerClearOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var book = AddBookWithTwoReviewsToDatabase(context);
            context.ChangeTracker.Clear();

            //ATTEMPT
            var readBook = context.Books.Single(x => x.BookId == book.BookId);

            //VERIFY
            _output.WriteLine($"Book has {readBook.Reviews?.Count.ToString() ?? "null"} reviews");
        }








        [Fact]
        public void TestCreateBookAndReadBackConfirmSameReferenceOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var book = AddBookWithTwoReviewsToDatabase(context);
            book.PromotionalText.ShouldBeNull();
            book.AddPromotion(123, "Added promotion");

            //ATTEMPT
            var readBook = context.Books.Single(x => x.BookId == book.BookId);

            //VERIFY
            ReferenceEquals(book, readBook).ShouldBeTrue();
            book.PromotionalText.ShouldEqual("Added promotion");
        }








        [Fact]
        public void TestCreateBookAndReadBackBeforeChangeTrackerClearOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            Book book;
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();

                book = AddBookWithTwoReviewsToDatabase(context);
            }
            //ATTEMPT
            using (var context = new BookDbContext(options))
            {
                var readBook = context.Books.Single(x => x.BookId == book.BookId);

                //VERIFY
                _output.WriteLine($"Book has {readBook.Reviews?.Count.ToString() ?? "null"} reviews");
            }
        }















        private static Book AddBookWithTwoReviewsToDatabase(BookDbContext context)
        {
            var book = new Book("test title", new DateTime(2000, 1, 2), false,
                "publisher", 123, "imageurl",
                new List<Author> {new Author("Author1", null)},
                new List<Tag> {new Tag("Tag1")},
                new List<byte> {1, 2}, "reviewUser");
            context.Add(book);
            context.SaveChanges();
            return book;
        }
    }
}