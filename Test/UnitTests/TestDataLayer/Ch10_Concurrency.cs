// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter10Listings.EfClasses;
using Test.Chapter10Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_Concurrency
    {
        public Ch10_Concurrency(ITestOutputHelper output)
        {
            _output = output;

            _options = this.CreateUniqueClassOptions<ConcurrencyDbContext>();
            using (var context = new ConcurrencyDbContext(_options))
            {
                context.Database.EnsureCreated();
                if (!context.Books.Any())
                {
                    context.Books.Add(new ConcurrencyBook
                    {
                        Title = "Default Book",
                        PublishedOn = new DateTime(2015,1,1),
                        Author = new ConcurrencyAuthor { Name = "Default Author" }
                    });
                    context.SaveChanges();
                }
            }
        }

        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<ConcurrencyDbContext> _options;

        private static string BookSaveChangesWithChecks //#A
            (ConcurrencyDbContext context)
        {
            string error = null;
            try
            {
                context.SaveChanges(); //#B
            }
            catch (DbUpdateConcurrencyException ex) //#C
            {
                var entry = ex.Entries.Single(); //#D
                error = HandleBookConcurrency( //#E
                    context, entry); //#E
                if (error == null)
                    context.SaveChanges(); //#F
            }
            return error; //#G
        }
        /***********************************************************
        #A This method is called after the Book entity has been updated in some way 
        #B I call SaveChanges within a try...catch so that I can catch a DbUpdateConcurrencyException if it occurs
        #C I catch the DbUpdateConcurrencyException and put in my code to handle it
        #D In this case you know only one Book will be updated. In other cases you might need to handle multiple entities.
        #E I call my HandleBookConcurrency method, which returns null if the error was handled, or an error message if it wasn't handled
        #F If the conflict was handled then I need to call SaveChanges to update the Book
        #G I return the error message, or null if there was no error
         * **********************************************************/

        private static string HandleBookConcurrency(                 //#A
            DbContext context, 
            EntityEntry entry)
        {
            var book = entry.Entity 
                as ConcurrencyBook;
            if (book == null)                                        //#B
                throw new NotSupportedException(
        "Don't know how to handle concurrency conflicts for " +
                    entry.Metadata.Name);

            var whatTheDatabaseHasNow =                              //#C
                context.Set<ConcurrencyBook>().AsNoTracking()        //#D
                    .SingleOrDefault(p => p.ConcurrencyBookId
                        == book.ConcurrencyBookId);
            if (whatTheDatabaseHasNow == null)                       //#E
                return "Unable to save changes.The book was deleted by another user.";

            var someoneElsesData =                                   //#F
                context.Entry(whatTheDatabaseHasNow);                //#F

            foreach (var property in entry.Metadata.GetProperties()) //#G
            {
                var theOriginalValue = entry                        //#H
                    .Property(property.Name).OriginalValue;          //#H
                var someoneElseValue = someoneElsesData          //#I
                    .Property(property.Name).CurrentValue;           //#I
                var whatIWantedItToBe = entry                      //#J
                    .Property(property.Name).CurrentValue;           //#J

                // TODO: Logic to decide which value should be written to database
                if (property.Name ==                                 //#K
                    nameof(ConcurrencyBook.PublishedOn))             //#K
                {
                    entry.Property(property.Name).CurrentValue =     //#K
                    //… your code to pick which PublishedOn to use   //#K
                        new DateTime(2050, 5, 5); //!!!!!! leave out
                }                                                    //#K

                entry.Property(property.Name).OriginalValue =        //#L
                    someoneElsesData.Property(property.Name)         //#L
                        .CurrentValue;                               //#L
            }
            return null;                                             //#M
        }
        /******************************************************************
        #A Takes in the application’s DbContext and the ChangeTracking entry from the exception’s Entities property.
        #B Handles only ConcurrencyBook, so throws an exception if the entry isn’t of type Book
        #C You want to get the data that someone else wrote into the database after your read. 
        #D Entity must be read as NoTracking; otherwise, it’ll interfere with the same entity you’re trying to write.
        #E Concurrency conflict method doesn't handle the case where the book was deleted, so it returns a user-friendly error message.
        #F You get the TEntity version of the entity, which has all the tracking information.
        #G You go through all the properties in the book entity to reset the Original values so that the exception doesn't happen again.
        #H Holds the version of the property at the time you did the tracked read of the book.
        #I Holds the version of the property as written to the database by someone else.
        #J Holds the version of the property that you wanted to set it to in your update.
        #K Here you set the OriginalValue to the value that someone else set it to. This handles using concurrency tokens or a timestamp.
        #L Business logic to handle PublishedOn: either set to your value, or the other person's value, or throw exception.
        #M You return null to say you handled this concurrency issue.
         *****************************************************************/

        [Fact]
        public void CreateConcurrencyDataAllOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                var numBooks = context.Books.Count();

                //ATTEMPT
                context.Books.Add(new ConcurrencyBook
                {
                    Title = "Unit Test",
                    PublishedOn = new DateTime(2014, 1, 1),
                    Author = new ConcurrencyAuthor { Name = "Unit Test"}
                });
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(numBooks + 1);
            }
        }

        [Fact]
        public void HandleExceptionOnPublishedDateChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var firstBook = context.Books.First(); //#A

                context.Database.ExecuteSqlRaw(
                    "UPDATE dbo.Books SET PublishedOn = GETDATE()" +  //#B
                    " WHERE ConcurrencyBookId = @p0",                  //#B
                    firstBook.ConcurrencyBookId);                      //#B
                firstBook.Title = Guid.NewGuid().ToString(); //#C
                var error = BookSaveChangesWithChecks(context);

                //VERIFY
                error.ShouldBeNull();
            }
            
            using (var context = new ConcurrencyDbContext(_options))
            {
                var rereadBook = context.Books.First();
                rereadBook.PublishedOn.ShouldEqual(new DateTime(2050, 5, 5));
            }
        }

        [Fact]
        public void ProduceErrorOnBookDeletedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var firstBook = context.Books.First();

                context.Database.ExecuteSqlRaw(
                    "DELETE dbo.Books WHERE ConcurrencyBookId = @p0",
                    firstBook.ConcurrencyBookId);
                firstBook.Title = Guid.NewGuid().ToString();
                var error = BookSaveChangesWithChecks(context);
                //VERIFY
                error.ShouldEqual("Unable to save changes.The book was deleted by another user.");
            }
        }

        [Fact]
        public void ShowGetDatabaseValuesOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT
                var firstBook = context.Books.First();
                firstBook.Title = "New Title";

                var databaseValues = (ConcurrencyBook) context.Entry(firstBook).GetDatabaseValues().ToObject();

                //VERIFY
                databaseValues.Title.ShouldNotEqual(firstBook.Title);
            }
        }

        [Fact]
        public void ThrowExceptionOnAuthorChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT


                var firstAuthor = context.Authors.First(); //#A
                context.Database.ExecuteSqlRaw(      //#B
                    "UPDATE dbo.Authors SET Name = @p0"+ //#B
                    " WHERE ConcurrencyAuthorId = @p1",  //#B
                    firstAuthor.Name,                    //#B
                    firstAuthor.ConcurrencyAuthorId);    //#B
                firstAuthor.Name = "Concurrecy Name"; //#C
                //context.SaveChanges(); //#D
                /******************************************
                #A I load the first author in the database as a tracked entity
                #B I simulate another thread/application updating the entity. In fact nothing is changed, but the timestamp
                #C I change something in the author to cause EF Core to do an update to the book
                #D This SaveChanges will throw an DbUpdateConcurrencyException
                 * ***************************************/

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
                ////to get the logs you need to fail see https://github.com/aspnet/Tooling/issues/541
                //Assert.True(false, "failed the test so that the logs show");
            }
        }

        [Fact]
        public void ThrowExceptionOnPublishedDateChangedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT

                var firstBook = context.Books.First(); //#A

                context.Database.ExecuteSqlRaw(
                    "UPDATE dbo.Books SET PublishedOn = GETDATE()" + //#B
                    " WHERE ConcurrencyBookId = @p0",                //#B
                    firstBook.ConcurrencyBookId);                    //#B
                firstBook.Title = Guid.NewGuid().ToString(); //#C
                //context.SaveChanges(); //#D
                /******************************************
                #A I load the first book in the database as a tracked entity
                #B I simulate another thread/application changing the PublishedOn column of the same book
                #C I change the title in the book to cause EF Core to do an update to the book
                #D This SaveChanges will throw an DbUpdateConcurrencyException
                 * ***************************************/

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void ThrowExceptionRowDeletedOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {
                //ATTEMPT

                var firstBook = context.Books.First();

                context.Database.ExecuteSqlRaw(
                    "DELETE dbo.Books WHERE ConcurrencyBookId = @p0", 
                    firstBook.ConcurrencyBookId);
                firstBook.Title = Guid.NewGuid().ToString();

                var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

                //VERIFY
                ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s). Data may have been modified or deleted since entities were loaded. ")
                    .ShouldBeTrue();
            }
        }

        [Fact]
        public void UpdateBookTitleOk()
        {
            //SETUP
            using (var context = new ConcurrencyDbContext(_options))
            {

                var firstBookId = context.Books.First().ConcurrencyBookId;

                //ATTEMPT
                var firstBook = context.Books.First(k => k.ConcurrencyBookId == firstBookId);
                var sqlTitle = Guid.NewGuid().ToString();
                var newDate = DateTime.Now.AddDays(100);
                context.Database.ExecuteSqlRaw(
                    "UPDATE dbo.Books SET Title = @p0 WHERE ConcurrencyBookId = @p1", 
                    sqlTitle, firstBookId);
                firstBook.PublishedOn = newDate;
                context.SaveChanges();

                //VERIFY
                context.Entry(firstBook).Reload();
                firstBook.Title.ShouldEqual(sqlTitle);
                firstBook.PublishedOn.ShouldEqual(newDate);

                //foreach (var log in logIt.Logs)
                //{
                //    _output.WriteLine(log);
                //}
                ////to get the logs you need to fail see https://github.com/aspnet/Tooling/issues/541
                //Assert.True(false, "failed the test so that the logs show");
            }
        }

        /***********************************************************
        #A My method takes in the application DbContext and the ChangeTracking entry from the exception's Entities property
        #B This method only handles a ConcurrecyBook, so throws an exception if the entry isn't of type Book
        #C I want to get the data that someone else wrote into the database after my read. 
        #D This entity MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
        #E This concurrency conflict method does not handle the case where the book was deleted, so it returns a user-friendly error message
        #F I get the TEntity version of the entity, which has all the tracking information
        #G In this case I go through all the properties in the book entity. I need to do this to reset the Original values so that the exception does not happen again
        #H This holds the version of the property at the time when I did the tracked read of the book
        #I This holds the version of the property as written to the database by someone else
        #J This holds the version of the property that I wanted to set it to in my update
        #K This is where you should put your code to fix the concurrency issue. I set the PublishedOn property to a specific value so I can check it in my unit test
        #L Here I set the OriginalValue to the value that someone else set it to. This handles both the case where you use concurrency tokens or a timestamp.
        #M I return null to say I handled this concurrency issue
         * ********************************************************/
    }
}