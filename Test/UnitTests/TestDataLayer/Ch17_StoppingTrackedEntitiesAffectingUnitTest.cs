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
    public class Ch17_StoppingTrackedEntitiesAffectingUnitTest
    {
        private readonly ITestOutputHelper _output;

        public Ch17_StoppingTrackedEntitiesAffectingUnitTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ExampleIdentityResolutionBad()
        {
            //SETUP
            var options = SqliteInMemory
                .CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);

            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var book = context.Books.First();
            book.Price = 123;
            // Should call context.SaveChanges()

            //VERIFY
            var verifyBook = context.Books.First();
            //!!! THIS IS WRONG !!! THIS IS WRONG
            verifyBook.Price.ShouldEqual(123);
        }

        [Fact]
        public void INCORRECTtestOfDisconnectedState()
        {
            //SETUP
            var options = SqliteInMemory
                .CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);

            context.Database.EnsureCreated();  //#A
            context.SeedDatabaseFourBooks();   //#A

            //ATTEMPT
            var book = context.Books            //#B
                .OrderBy(x => x.BookId).Last(); //#B
            book.Reviews.Add(new Review { NumStars = 5 });  //#C
            context.SaveChanges();                      //#D

            //VERIFY
            //THIS IS INCORRECT!!!!!
            context.Books                      //#E
                .OrderBy(x => x.BookId).Last() //#E
                .Reviews.Count.ShouldEqual(3); //#E
        }
        /********************************************************************
        #A Sets up the test database with test data consisting of four books
        #B Reads in the last book from your test set, which you know has two reviews
        #C Adds another review to the book. This shouldn’t work, but it does because the seed data is still being tracked by the DbContext instance.
        #D Saves it to the database
        #E Checks that you have three reviews, which works, but the unit test should have FAILED with an exception earlier
         ********************************************************************/

        [Fact]
        public void UsingChangeTrackerClear()
        {
            //SETUP
            var options = SqliteInMemory
                .CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);

            context.Database.EnsureCreated();               //#A
            context.SeedDatabaseFourBooks();                //#A

            context.ChangeTracker.Clear();                  //#B

            //ATTEMPT
            var book = context.Books                        //#C
                .Include(b => b.Reviews)
                .OrderBy(b => b.BookId).Last();             //#C
            book.Reviews.Add(new Review { NumStars = 5 });  //#D

            context.SaveChanges();                          //#E

            //VERIFY
            context.ChangeTracker.Clear();                  //#B

            context.Books.Include(b => b.Reviews)           //#F
                .OrderBy(x => x.BookId).Last()              //#F
                .Reviews.Count.ShouldEqual(3);              //#F
        }
        /*************************************************************************
        #A Sets up the test database with test data consisting of four books
        #B Call ChangeTracker.Clear to stop tracking all entities. 
        #C Reads in the last book from your test set, which you know has two reviews
        #D When you try to add the new Review, EF Core throws a NullReferenceException because the Book’s Review collection isn’t loaded and is therefore null.
        #E Saves it to the database
        #F This reloads the book with its reviews to check there are 3 Reviews
         *************************************************************************/

        [Fact]
        public void UsingThreeInstancesOfTheDbcontext()
        {
            //SETUP
            var options = SqliteInMemory          //#A
                .CreateOptions<EfCoreContext>();  //#A
            options.StopNextDispose();   //#B
            using (var context = new EfCoreContext(options))   //#C
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();    //#D
            }
            options.StopNextDispose();   //#B
            using (var context = new EfCoreContext(options))      //#E
            {
                //ATTEMPT
                var book = context.Books //#F
                    .Include(x => x.Reviews)
                    .OrderBy(x => x.BookId).Last(); //#F
                book.Reviews.Add(new Review { NumStars = 5 });  //#G

                context.SaveChanges();      //#H
            }
            using (var context = new EfCoreContext(options))      //#E
            {
                //VERIFY
                context.Books.Include(b => b.Reviews)   //#F
                    .OrderBy(x => x.BookId).Last()      //#F
                    .Reviews.Count.ShouldEqual(3);      //#F
            }
        }

        /*************************************************************************
        #A Creates the in-memory SQLite options in the same way as the preceding example
        #B This stops the SQLite connection being disposed after the first instance of the application’s DbContext
        #C Creates the first instance of the application’s DbContext
        #D Sets up the test database with test data consisting of four books, but this time in a separate DbContext instance
        #E Closes that last instance and opens a new instance of the application’s DbContext. The new instance doesn’t have any tracked entities that could alter how the test runs.
        #F Reads in the last book from your test set, which you know has two reviews
        #G When you try to add the new Review, EF Core throws a NullReferenceException because the Book’s Review collection isn’t loaded and is therefore null.
        #H Call SaveChanges to update the database
        #F This reloads the book with its reviews to check there are 3 Reviews
         *************************************************************************/
    }
}