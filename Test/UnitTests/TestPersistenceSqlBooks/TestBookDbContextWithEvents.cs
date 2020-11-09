// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Infrastructure.Books.CachedValues.Handlers;
using BookApp.Persistence.EfCoreSql.Books;
using GenericEventRunner.ForSetup;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceSqlBooks
{
    public class TestBookDbContextWithEvents
    {
        [Fact]
        public void TestBookDbContextAddReviewEventOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[0].AddReview(5, "comment", "me");

            //VERIFY
            var beforeEvent = books[0].GetBeforeSaveEventsThenClear().Single();
            beforeEvent.ShouldBeType<BookReviewAddedEvent>();
            books[0].GetAfterSaveEventsThenClear().Count.ShouldEqual(0);
        }

        [Fact]
        public void TestBookDbContextRemoveReviewEventOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[3].RemoveReview(1);

            //VERIFY
            var beforeEvent = books[3].GetBeforeSaveEventsThenClear().Last();
            beforeEvent.ShouldBeType<BookReviewRemovedEvent>();
            books[3].GetAfterSaveEventsThenClear().Count.ShouldEqual(0);
        }

        [Fact]
        public void TestBookDbContextAddReviewCacheUpdatedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            var eventConfig = new GenericEventRunnerConfig
            {
                NotUsingDuringSaveHandlers = true
            };
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>(null, eventConfig);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[0].AddReview(5, "comment", "me");
            context.SaveChanges();

            //VERIFY
            context.ChangeTracker.Clear();
            var book = context.Books.Single(x => x.BookId == books[0].BookId);
            book.ReviewsCount.ShouldEqual(1);
            book.ReviewsAverageVotes.ShouldEqual(5);
        }

        [Fact]
        public void TestBookDbContextRemoveReviewCacheUpdatedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            var eventConfig = new GenericEventRunnerConfig
            {
                NotUsingDuringSaveHandlers = true
            };
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>(null, eventConfig);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[3].RemoveReview(1);
            context.SaveChanges();

            //VERIFY
            context.ChangeTracker.Clear();
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[3].BookId);
            book.ReviewsCount.ShouldEqual(1);
            book.ReviewsAverageVotes.ShouldEqual(3);
        }

        [Fact]
        public void TestBookDbContextNewAuthorDoesNotCreateEventOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var author = new Author("Name1", null);
            author.Name = "Name2";

            //VERIFY
            author.GetBeforeSaveEventsThenClear().Count.ShouldEqual(0);
        }

        [Fact]
        public void TestBookDbContextAlterAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            var eventConfig = new GenericEventRunnerConfig
            {
                NotUsingDuringSaveHandlers = true
            };
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>(null, eventConfig);
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[0].AuthorsLink.Single().Author.Name = "Test Name";
            context.SaveChanges();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBooks = context.Books.ToList();
            readBooks[0].AuthorsOrdered.ShouldEqual("Test Name");
            readBooks[1].AuthorsOrdered.ShouldEqual("Test Name");
            readBooks[2].AuthorsOrdered.ShouldNotEqual("Test Name");
            readBooks[3].AuthorsOrdered.ShouldNotEqual("Test Name");
        }

        [Fact]
        public void TestGenericEventRunnerConfigAddActionToRunAfterDetectChangeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            var eventConfig = new GenericEventRunnerConfig();
            eventConfig.AddActionToRunAfterDetectChanges<BookDbContext>(localContext =>
                localContext.ChangeChecker());
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>(null, eventConfig);
            context.Database.EnsureCreated();

            //ATTEMPT
            var books = context.SeedDatabaseFourBooks();

            //VERIFY
            var timeNow = DateTime.UtcNow;
            books.ForEach(x => x.LastUpdatedUtc.ShouldBeInRange(DateTime.UtcNow.AddMilliseconds(-500), timeNow));
            books.ForEach(x => x.NotUpdatedYet.ShouldBeTrue());
        }

    }
}