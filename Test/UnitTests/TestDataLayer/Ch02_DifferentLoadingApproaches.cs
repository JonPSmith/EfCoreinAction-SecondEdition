// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
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
        public void TestAsNoTrackingBookCountAuthorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var books = context.Books
                .AsNoTracking()
                .Include(book => book.AuthorsLink) 
                .ThenInclude(book => book.Author)
                .ToList();

            //VERIFY
            books.SelectMany(x => x.AuthorsLink.Select(y => y.Author)).Distinct().Count().ShouldEqual(4);
        }

        [Fact]
        public void TestBookCountAuthorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var books = context.Books
                .Include(book => book.AuthorsLink)
                .ThenInclude(bookAuthor => bookAuthor.Author)
                .ToList();

            //VERIFY
            books.Count.ShouldEqual(4);
            books.SelectMany(x => x.AuthorsLink.Select(y => y.Author)).Distinct().Count().ShouldEqual(3);
        }

        [Fact]
        public void TestEagerLoadBookAllOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var firstBook = context.Books
                .Include(book => book.AuthorsLink) //#A
                .ThenInclude(bookAuthor => bookAuthor.Author) //#B                    
                .Include(book => book.Reviews) //#C
                .Include(book => book.Tags) //#D
                .Include(book => book.Promotion) //#E
                .First(); //#F
            /*********************************************************
                #A The first Include() gets a collection of BookAuthor
                #B The ThenInclude() gets the next link, in this case the link to the Author
                #C The Include() gets a collection of Reviews, which may be an empty collection
                #D This loads the Tags. Note that this directly accesses the Tags
                #E This loads any optional PriceOffer class, if one is assigned
                #F This takes the first book
                * *******************************************************/

            //VERIFY
            firstBook.AuthorsLink.ShouldNotBeNull();
            firstBook.AuthorsLink.First()
                .Author.ShouldNotBeNull();

            firstBook.Reviews.ShouldNotBeNull();
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
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showlog = true;
            var firstBook = context.Books
                .Include(book => book.Reviews) //#A
                .First(); //#B
            /*********************************************************
                #A The Include() gets a collection of Reviews, which may be an empty collection
                #B This takes the first book
                * *******************************************************/

            //VERIFY
            firstBook.Reviews.ShouldNotBeNull();
            firstBook.AuthorsLink.ShouldBeNull();
        }

        [Fact]
        public void TestEagerLoadIncludeNotNeededOk()
        {
            //SETUP
            var logs = new List<LogOutput>();
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log => logs.Add(log));
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var newPrices = context.Books.Include(x => x.Promotion)
                .Select(x => (decimal?) x.Promotion.NewPrice)
                .ToArray();

            //VERIFY
            newPrices.ShouldEqual(new decimal?[]{null, null, null, 219} );
        }

        [Fact]
        public void TestExplicitLoadBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var firstBook = context.Books.First();            //#A
            context.Entry(firstBook)
                .Collection(book => book.AuthorsLink).Load(); //#B
            foreach (var authorLink in firstBook.AuthorsLink) //#C
            {
                //#C
                context.Entry(authorLink)                     //#C
                    .Reference(bookAuthor =>                  //#C
                        bookAuthor.Author).Load();            //#C
            }                                                 //#C

            context.Entry(firstBook)                          //#D
                .Collection(book => book.Reviews).Load();     //#D
            context.Entry(firstBook)  //#E
                .Collection(book => book.Tags).Load(); //#E
            context.Entry(firstBook)                          //#F
                .Reference(book => book.Promotion).Load();    //#F
            /*********************************************************
                #A This reads in the first book on its own
                #B This explicitly loads the linking table, BookAuthor
                #C To load all the possible Authors it has to loop through all the BookAuthor entries and load each linked Author class
                #D This loads all the Reviews
                #E This loads the Tags
                #F This loads the optional PriceOffer class
                * *******************************************************/

            //VERIFY
            firstBook.AuthorsLink.ShouldNotBeNull();
            firstBook.AuthorsLink.First()
                .Author.ShouldNotBeNull();

            firstBook.Reviews.ShouldNotBeNull();
        }

        [Fact]
        public void TestExplicitLoadWithQueryBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var firstBook = context.Books.First(); //#A
            var numReviews = context.Entry(firstBook) //#B
                .Collection(book => book.Reviews) //#B
                .Query().Count(); //#B
            var starRatings = context.Entry(firstBook) //#C
                .Collection(book => book.Reviews) //#C
                .Query().Select(review => review.NumStars) //#C
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


        [Fact]
        public void TestReadJustBookDisconnectedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books.First();

            //VERIFY
            book.AuthorsLink.ShouldBeNull();
            book.Reviews.ShouldBeNull();
            book.Promotion.ShouldBeNull();
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
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showlog = true;
            var books = context.Books
                .Select(book => new //#A
                    {
                        //#A
                        book.Title, //#B
                        book.Price, //#B
                        NumReviews //#C
                            = book.Reviews.Count, //#C
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