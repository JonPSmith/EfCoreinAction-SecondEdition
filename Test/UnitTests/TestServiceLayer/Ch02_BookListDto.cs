// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.BookServices;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch02_BookListDto
    {
        public Ch02_BookListDto(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;


        [Fact]
        public void TestAverageDirectOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showLog = true;
            var averages = context.Books.Select(p =>
                p.Reviews.Count == 0 ? null : (decimal?) p.Reviews.Select(q => q.NumStars).Average()).ToList();

            //VERIFY
            averages.Count(x => x == null).ShouldEqual(3);
            averages.Count(x => x != null).ShouldEqual(1);
        }

        [Fact]
        public void TestAverageOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var dtos = context.Books.Select(p => new
            {
                NumReviews = p.Reviews.Count,
                ReviewsAverageVotes = p.Reviews.Count == 0 ? null : (double?) p.Reviews.Average(q => q.NumStars)
            }).ToList();

            //VERIFY
            dtos.Any(x => x.ReviewsAverageVotes > 0).ShouldBeTrue();
        }

        [Fact]
        public void TestDirectSelectBookListDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var dtos = context.Books.Select(p => new BookListDto
            {
                BookId = p.BookId,
                Title = p.Title,
                Price = p.Price,
                PublishedOn = p.PublishedOn,
            }).ToList();

            //VERIFY
            dtos.Last().BookId.ShouldNotEqual(0);
            dtos.Last().Title.ShouldNotBeNull();
            dtos.Last().Price.ShouldNotEqual(0);
        }

        [Fact]
        public void TestEagerBookListDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var firstBook = context.Books
                .Include(r => r.AuthorsLink)
                .ThenInclude(r => r.Author)
                .Include(r => r.Reviews)
                .Include(r => r.Promotion)
                .First();
            var dto = new BookListDto
            {
                BookId = firstBook.BookId,
                Title = firstBook.Title,
                Price = firstBook.Price,
                ActualPrice = firstBook //#A
                                  .Promotion?.NewPrice //#A
                              ?? firstBook.Price, //#A
                PromotionPromotionalText = firstBook //#A
                    .Promotion?.PromotionalText, //#A
                AuthorsOrdered = string.Join(", ", //#B
                    firstBook.AuthorsLink //#B
                        .OrderBy(l => l.Order) //#B
                        .Select(a => a.Author.Name)), //#B
                ReviewsCount = firstBook.Reviews.Count, //#C
                //The test on there being any reviews is needed because of bug in EF Core V2.0.0, issue #9516
                ReviewsAverageVotes = //#D
                    firstBook.Reviews.Count == 0 //#D
                        ? null //#D
                        : (double?) firstBook.Reviews //#D
                            .Average(q => q.NumStars) //#D
            };

            //VERIFY
            dto.BookId.ShouldNotEqual(0);
            dto.Title.ShouldNotBeNull();
            dto.Price.ShouldNotEqual(0);
            dto.AuthorsOrdered.ShouldNotBeNull();
            /*********************************************************
                #A Notice the use of ?. This returns null if Promotion is null, otherwise it returns the property
                #B This orders the authors' names by the Order and then extracts their names
                #C We simply count the number of reviews
                #D EF Core turns the LINQ average into the SQL AVG command that runs on the database
                * *******************************************************/
        }

        [Fact]
        public void TestIndividualBookListDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var titles = context.Books.Select(p => p.Title);
            var orgPrices = context.Books.Select(p => p.Price);
            var actuaPrice = context.Books.Select(p =>
                p.Promotion == null
                    ? p.Price
                    : p.Promotion.NewPrice);
            var pText = context.Books.Select(p =>
                p.Promotion == null
                    ? null
                    : p.Promotion.PromotionalText);
            var authorOrdered =
                context.Books.Select(p =>
                    string.Join(", ",
                        p.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)));
            var reviewsCount = context.Books.Select(p => p.Reviews.Count);
            //The test on there being any reviews is needed because of bug in EF Core V2.0.0, issue #9516
            var reviewsAverageVotes = context.Books.Select(p =>
                p.Reviews.Count == 0
                    ? null
                    : (double?) p.Reviews.Average(q => q.NumStars));

            //VERIFY
            titles.ToList();
            orgPrices.ToList();
            actuaPrice.ToList();
            pText.ToList();
            authorOrdered.ToList();
            reviewsCount.ToList();
            reviewsAverageVotes.ToList();
        }

        [Fact]
        public void TestIQueryableSelectBookListDtoOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var dtos = context.Books.MapBookToDto().OrderByDescending(x => x.BookId).ToList();

            //VERIFY
            dtos.First().BookId.ShouldNotEqual(0);
            dtos.First().Title.ShouldNotBeNull();
            dtos.First().Price.ShouldNotEqual(0);
            dtos.First().ActualPrice.ShouldNotEqual(dtos.Last().Price);
            dtos.First().AuthorsOrdered.Length.ShouldBeInRange(1, 100);
            dtos.First().ReviewsCount.ShouldEqual(2);
            dtos.First().ReviewsAverageVotes.ShouldEqual(5);
        }

        [Fact]
        public void TestRawSqlOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var books =
                context.Books.FromSqlRaw(
                        "SELECT * FROM Books AS a ORDER BY (SELECT AVG(b.NumStars) FROM Review AS b WHERE b.BookId = a.BookId) DESC")
                    .ToList();

            //VERIFY
            books.First().Title.ShouldEqual("Quantum Networking");
        }
    }
}