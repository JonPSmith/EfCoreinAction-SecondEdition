// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_AddEntityWithRelationships
    {
        private readonly ITestOutputHelper _output;

        public Ch06_AddEntityWithRelationships(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestWrongWayToSetupRelationshipsOk()
        {
            //SETUP
            var sqlOptions = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(sqlOptions))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new Book { Title = "Test", Price = 10m };
                context.Add(book);
                context.SaveChanges();
                var review = new Review { BookId = book.BookId, NumStars = 1 };
                context.Add(review);
                context.SaveChanges();

                //VERIFY
                var bookWithReview = context.Books.Include(x => x.Reviews).Single();
                bookWithReview.Reviews.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestRightWayToSetupRelationshipsOk()
        {
            //SETUP
            var showlog = false;
            var options = this.CreateUniqueClassOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureClean();

                showlog = true;
                //ATTEMPT
                var book = new Book                  //#A
                {                                    //#A
                    Title = "Test",                  //#A
                    Reviews = new List<Review>()     //#A
                };                                   //#A
                book.Reviews.Add(                    //#B
                    new Review { NumStars = 1 });    //#B
                context.Add(book);                   //#C
                context.SaveChanges();               //#D
                /*********************************************************
                #A This creates a new Book
                #B This adds a new Review to the Book's Reviews navigational property
                #C The Add method says that the entity instance should be Added to the appropriate row, with any relationships either added or updated
                #D The SaveChanges carries out the database update
                 *********************************************************/
                showlog = false;

                //VERIFY
                var bookWithReview = context.Books.Include(x => x.Reviews).Single();
                bookWithReview.Reviews.Count.ShouldEqual(1);
            }
        }

        [Fact]
        public void TestWhatHappensIfRelationshipLinkAndForeignKeyAreBothSetOk()
        {
            //SETUP
            var sqlOptions = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(sqlOptions))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var book1 = new Book
                {
                    Title = "Test1",
                    Reviews = new List<Review>()
                };
                var book2 = new Book
                {
                    Title = "Test2",
                };
                context.Add(book2);
                context.SaveChanges();

                //This review has its FK set to book2, but its added to book1
                var review = new Review { BookId = book2.BookId, NumStars = 1 };
                book1.Reviews.Add(review);
                context.Add(book1);
                context.SaveChanges();

                //VERIFY
                var books = context.Books.Include(x => x.Reviews).OrderBy(x => x.Title).ToList();
                //This shows that adding the review to book1's Reviews navigational collection wins over the FK being set.
                books.Select(x => x.Reviews.Count).ShouldEqual(new[] { 1, 0 });
            }
        }
    }
}