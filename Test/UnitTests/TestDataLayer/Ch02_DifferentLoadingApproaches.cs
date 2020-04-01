// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter02Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_DifferentLoadingApproaches
    {
        public Ch02_DifferentLoadingApproaches(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public void TestEagerLoadBookAllOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books
                    .Include(r => r.AuthorsLink) //#A
                    .ThenInclude(r => r.Author) //#B
                    .Include(r => r.Reviews) //#C
                    .Include(r => r.Promotion) //#D
                    .First(); //#E
                /*********************************************************
                #A The first Include() gets a collection of BookAuthor
                #B The ThenInclude() gets the next link, in this case the link to the Author
                #C The Include() gets a collection of Reviews, which may be an empty collection
                #D This loads any optional PriceOffer class, if one is assigned
                #E This takes the first book
                * *******************************************************/

                //VERIFY
                book.AuthorsLink.ShouldNotBeNull();
                book.AuthorsLink.First()
                    .Author.ShouldNotBeNull();

                book.Reviews.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestEagerLoadBookAndReviewOk()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                showlog = true;
                var book = context.Books
                    .Include(r => r.Reviews) //#A
                    .First(); //#B
                /*********************************************************
                #A The Include() gets a collection of Reviews, which may be an empty collection
                #B This takes the first book
                * *******************************************************/

                //VERIFY
                book.Reviews.ShouldNotBeNull();
                book.AuthorsLink.ShouldBeNull();
            }
        }

        [Fact]
        public void TestEagerLoadIncludeNotNeededOk()
        {
            //SETUP
            var logs = new List<LogOutput>();
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log => logs.Add(log));
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var newPrices = context.Books.Include(x => x.Promotion)
                    .Select(x => (decimal?) x.Promotion.NewPrice)
                    .ToArray();

                //VERIFY
                newPrices.ShouldEqual(new decimal?[]{null, null, null, 219} );
            }
        }

        [Fact]
        public void TestExplicitLoadBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books.First(); //#A
                context.Entry(book)
                    .Collection(c => c.AuthorsLink).Load(); //#B
                foreach (var authorLink in book.AuthorsLink) //#C
                {
                    //#C
                    context.Entry(authorLink) //#C
                        .Reference(r => r.Author).Load(); //#C
                } //#C

                context.Entry(book) //#D
                    .Collection(c => c.Reviews).Load(); //#D
                context.Entry(book) //#E
                    .Reference(r => r.Promotion).Load(); //#E
                /*********************************************************
                #A This reads in the first book on its own
                #B This explicitly loads the linking table, BookAuthor
                #C To load all the possible Authors it has to loop through all the BookAuthor entries and load each linked Author class
                #D This loads all the Reviews
                #E This loads the optional PriceOffer class
                * *******************************************************/

                //VERIFY
                book.AuthorsLink.ShouldNotBeNull();
                book.AuthorsLink.First()
                    .Author.ShouldNotBeNull();

                book.Reviews.ShouldNotBeNull();
            }
        }

        [Fact]
        public void TestExplicitLoadWithQueryBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books.First(); //#A
                var numReviews = context.Entry(book) //#B
                    .Collection(c => c.Reviews) //#B
                    .Query().Count(); //#B
                var starRatings = context.Entry(book) //#C
                    .Collection(c => c.Reviews) //#C
                    .Query().Select(x => x.NumStars) //#C
                    .ToList(); //#C
                /*********************************************************
                #A This reads in the first book on its own
                #B This executes a query to count how many reviews there are for this book
                #C This executes a query to get all the star ratings for the book
                * *******************************************************/

                //VERIFY
                numReviews.ShouldEqual(0);
                starRatings.Count.ShouldEqual(0);
            }
        }


        [Fact]
        public void TestReadJustBookTableOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }

            using (var context = new EfCoreContext(options)
            ) //dispose first DbContext and create new one. That way the read isn't effected by the setup code
            {
                //ATTEMPT
                var book = context.Books.First();

                //VERIFY
                book.AuthorsLink.ShouldBeNull();
                book.Reviews.ShouldBeNull();
                book.Promotion.ShouldBeNull();
            }
        }

        [Fact]
        public void TestSelectLoadBookOk()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                showlog = true;
                var books = context.Books
                    .Select(p => new //#A
                        {
                            //#A
                            p.Title, //#B
                            p.Price, //#B
                            NumReviews //#C
                                = p.Reviews.Count, //#C
                        }
                    ).ToList();
                /*********************************************************
                #A This uses the LINQ select keyword and creates an anonymous type to hold the results
                #B These are simple copies of a couple of properties
                #C This runs a query that counts the number of reviews
                * *******************************************************/

                //VERIFY
                showlog = false;
                books.First().NumReviews.ShouldEqual(0);
            }
        }
    }
}