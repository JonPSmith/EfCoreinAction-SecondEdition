// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_DifferentLoadingApproaches
    {
        private readonly ITestOutputHelper _output;

        public Ch02_DifferentLoadingApproaches(ITestOutputHelper output)
        {
            _output = output;
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
            using (var context = new EfCoreContext(options)) //dispose first DbContext and create new one. That way the read isn't effected by the setup code
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
        public void TestEagerLoadBookAndReviewOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books
                    .Include(r => r.Reviews)             //#A
                    .First();                            //#B
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
        public void TestEagerLoadBookAllOk()
        {
            //SETUP
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books
                    .Include(r => r.AuthorsLink)         //#A
                         .ThenInclude(r => r.Author)     //#B

                    .Include(r => r.Reviews)             //#C
                    .Include(r => r.Promotion)           //#D
                    .First();                            //#E
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
        public void TestExplicitLoadBookOk()
        {
            //SETUP
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books.First();               //#A
                context.Entry(book)
                    .Collection(c => c.AuthorsLink).Load();//#B
                foreach (var authorLink in book.AuthorsLink)//#C
                {                                          //#C
                    context.Entry(authorLink)                   //#C
                        .Reference(r => r.Author).Load();  //#C
                }                                          //#C
                context.Entry(book)                             //#D
                    .Collection(c => c.Reviews).Load();    //#D
                context.Entry(book)                             //#E
                    .Reference(r => r.Promotion).Load();   //#E
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
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books.First();           //#A
                var numReviews = context.Entry(book)        //#B
                    .Collection(c => c.Reviews)             //#B
                    .Query().Count();                       //#B
                var starRatings = context.Entry(book)       //#C
                    .Collection(c => c.Reviews)             //#C
                    .Query().Select(x => x.NumStars)        //#C
                    .ToList();                              //#C
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
        public void TestSelectLoadBookOk()
        {
            //SETUP
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var books = context.Books
                    .Select(p => new              //#A
                        {                         //#A
                            p.Title,              //#B
                            p.Price,              //#B
                            NumReviews            //#C
                               = p.Reviews.Count, //#C
                        }
                    ).ToList();
                /*********************************************************
                #A This uses the LINQ select keyword and creates an anonymous type to hold the results
                #B These are simple copies of a couple of properties
                #C This runs a query that counts the number of reviews
                * *******************************************************/

                //VERIFY
                books.First().NumReviews.ShouldEqual(0);
            }
        }
    }
}