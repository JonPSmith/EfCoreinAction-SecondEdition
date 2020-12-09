// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter02Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_BookHashPerformance
    {
        private readonly ITestOutputHelper _output;

        public Ch02_BookHashPerformance(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookHashContextOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookHashContext>();
            using var context = new BookHashContext(options);
            context.Database.EnsureCreated();
            context.Add(CreateBookWithHashReviews(2));
            context.SaveChanges();

            context.ChangeTracker.Clear();

            var book= context.Books
                .Include(x => x.Reviews)
                .Single();

            //VERIFY

            book.Reviews.Count.ShouldEqual(2);
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookHashReviewQueryTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookHashContext>();
            using var context = new BookHashContext(options);
            context.Database.EnsureCreated();
            context.Add(CreateBookWithHashReviews(numReviews));
            context.SaveChanges();

            context.ChangeTracker.Clear();

            using(new TimeThings(_output, $"1: BookHashReview: {numReviews} entries"))
            {
                var book = context.Books
                    .Include(x => x.Reviews)
                    .Single();
                book.Reviews.Count.ShouldEqual(numReviews);
            }
            using (new TimeThings(_output, $"2: BookHashReview: {numReviews} entries"))
            {
                var book = context.Books
                    .Include(x => x.Reviews)
                    .Single();
                book.Reviews.Count.ShouldEqual(numReviews);
            }
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookListReviewQueryTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.Add(CreateBookWithListReviews(numReviews));
            context.SaveChanges();

            context.ChangeTracker.Clear();

            using (new TimeThings(_output, $"1: BookList: {numReviews} entries"))
            {
                var book = context.Books
                    .Include(x => x.Reviews)
                    .Single();
                book.Reviews.Count.ShouldEqual(numReviews);
            }
            using (new TimeThings(_output, $"2: BookList: {numReviews} entries"))
            {
                var book = context.Books
                    .Include(x => x.Reviews)
                    .Single();
                book.Reviews.Count.ShouldEqual(numReviews);
            }
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookHashReviewSaveChangesTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookHashContext>();
            using var context = new BookHashContext(options);
            context.Database.EnsureCreated();
            var book = CreateBookWithHashReviews(numReviews);
            using (new TimeThings(_output, $"ADD: BookHashReview: {numReviews} entries"))
                context.Add(book);
            context.SaveChanges();

            using (new TimeThings(_output, $"1: BookHashReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"2: BookHashReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookListReviewSaveChangesTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            var book = CreateBookWithListReviews(numReviews);
            using (new TimeThings(_output, $"ADD BookListReview: {numReviews} entries"))
                context.Add(book);
            context.SaveChanges();

            using (new TimeThings(_output, $"1: BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"2: BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookHashReviewSaveChangesWithAnotherBookTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookHashContext>();
            using var context = new BookHashContext(options);
            context.Database.EnsureCreated();
            var book1 = CreateBookWithHashReviews(numReviews);
            using (new TimeThings(_output, $"ADD1 BookListReview: {numReviews} entries"))
                context.Add(book1);
            var book2 = CreateBookWithHashReviews(numReviews);
            using (new TimeThings(_output, $"ADD2 BookListReview: {numReviews} entries"))
                context.Add(book2);

            using (new TimeThings(_output, $"Save(real): BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"Save nothing: BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
        }

        [Theory]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestBookListReviewSaveChangesWithAnotherBookTimeOk(int numReviews)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            var book1 = CreateBookWithListReviews(numReviews);
            using (new TimeThings(_output, $"ADD1 BookListReview: {numReviews} entries"))
                context.Add(book1);
            var book2 = CreateBookWithListReviews(numReviews);
            using (new TimeThings(_output, $"ADD2 BookListReview: {numReviews} entries"))
                context.Add(book2);

            using (new TimeThings(_output, $"Save(real): BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"Save nothing: BookListReview: {numReviews} entries"))
            {
                context.SaveChanges();
            }
        }

        private static Book CreateBookWithListReviews(int numReviews)
        {
            var book = new Book()
            {
                Reviews = new List<Review>()
            };
            for (int i = 0; i < numReviews; i++)
            {
                book.Reviews.Add(new Review());
            }

            return book;
        }

        private static BookHashReview CreateBookWithHashReviews(int numReviews)
        {
            var book = new BookHashReview()
            {
                Reviews = new HashSet<Review>()
            };
            for (int i = 0; i < numReviews; i++)
            {
                book.Reviews.Add(new Review());
            }

            return book;
        }
    }
}