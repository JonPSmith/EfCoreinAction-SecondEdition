// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Infrastructure.Books.CachedValues.ConcurrencyHandlers;
using BookApp.Infrastructure.Books.CachedValues.Handlers;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceNormalSqlBooks
{
    public class TestBookDbContextConcurrencyIssues
    {
        
        [Fact]
        public void TestBookDbContextAddReviewConcurrencyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>();
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[0].AddReview(3, "comment", "me");
            context.Database.ExecuteSqlInterpolated(
                $"UPDATE Books SET ReviewsCount = 2 WHERE BookId = {books[0].BookId}");
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                var status = e.HandleCacheValuesConcurrency(context);
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
                context.SaveChanges();
            }

            //VERIFY
            context.ChangeTracker.Clear();
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[0].BookId);
            book.ReviewsCount.ShouldEqual(3);
            book.ReviewsAverageVotes.ShouldEqual(1);
        }

        [Fact]
        public void TestBookDbContextAddReviewConcurrencySecondExceptionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>();
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[0].AddReview(4, "comment", "me");
            context.Database.ExecuteSqlInterpolated(
                $"UPDATE Books SET ReviewsCount = 2 WHERE BookId = {books[0].BookId}");
            try
            {
                context.SaveChanges();
            }
            catch (Exception e1)
            {
                var status1 = e1.HandleCacheValuesConcurrency(context);
                status1.IsValid.ShouldBeTrue(status1.GetAllErrors());
                try
                {
                    context.Database.ExecuteSqlInterpolated(
                        $"UPDATE Books SET ReviewsCount = 4 WHERE BookId = {books[0].BookId}");
                    context.SaveChanges();
                }
                catch (Exception e2)
                {
                    var status2 = e2.HandleCacheValuesConcurrency(context);
                    status2.IsValid.ShouldBeTrue(status2.GetAllErrors());
                    context.SaveChanges();
                }
            }

            //VERIFY
            context.ChangeTracker.Clear();
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[0].BookId);
            book.ReviewsCount.ShouldEqual(5);
            book.ReviewsAverageVotes.ShouldEqual(0.8);
        }

        [Fact]
        public void TestBookDbContextRemoveReviewConcurrencyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>();
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[3].RemoveReview(1);
            context.Database.ExecuteSqlInterpolated(
                $"UPDATE Books SET ReviewsCount = 1 WHERE BookId = {books[3].BookId}");
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                var status = e.HandleCacheValuesConcurrency(context);
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
                context.SaveChanges();
            }

            //VERIFY
            context.ChangeTracker.Clear();
            var book = context.Books.Include(x => x.Reviews).Single(x => x.BookId == books[3].BookId);
            book.ReviewsCount.ShouldEqual(0);
            book.ReviewsAverageVotes.ShouldEqual(0);
        }

        [Fact]
        public void TestBookDbContextAlterAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>();
            context.Database.EnsureCreated();
            var books = context.SeedDatabaseFourBooks();

            //ATTEMPT
            books[3].AuthorsLink.Single().Author.Name = "Test Name";
            context.Database.ExecuteSqlInterpolated(
                $"UPDATE Books SET AuthorsOrdered = 'bad name' WHERE BookId = {books[3].BookId}");
            try
            {
                context.SaveChanges();
            }
            catch (Exception e)
            {
                var status = e.HandleCacheValuesConcurrency(context);
                status.IsValid.ShouldBeTrue(status.GetAllErrors());
                context.SaveChanges();
            }

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = context.Books.Single(x => x.BookId == books[3].BookId);
            readBook.AuthorsOrdered.ShouldEqual("Test Name");
        }
    }
}