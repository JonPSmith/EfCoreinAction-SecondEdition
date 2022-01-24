// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.CommunityStandUp;

public class WaysToMimicApplicationDbContext
{
    [Fact]
    public void INCORRECTtestOfDisconnectedState()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();
        using var context = new BookDbContext(options);
        context.Database.EnsureClean();
        context.SeedDatabaseFourBooks();

        //ATTEMPT
        var book = context.Books
            .OrderBy(x => x.BookId).Last();
        book.AddReview(5, "great!", "me");

        //VERIFY
        //THIS IS INCORRECT!!!!!
        context.Books
            .OrderBy(x => x.BookId).Last()
            .Reviews.Count.ShouldEqual(3);
    }

    [Fact]
    public void MakeItFailByMakingEachStepHasItsOwnContext()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();
        using (var context = new BookDbContext(options))
        {
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();
        }

        //ATTEMPT
        using (var context = new BookDbContext(options))
        {
            var book = context.Books
                .OrderBy(x => x.BookId).Last();
            book.AddReview(5, "great!", "me");
        }

        //VERIFY
        using (var context = new BookDbContext(options))
        {
            context.Books
                .OrderBy(x => x.BookId).Last()
                .Reviews.Count.ShouldEqual(3);
        }
    }

    [Fact]
    public void FixTheProblemsEachStepHasItsOwnContext()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();
        using (var context = new BookDbContext(options))
        {
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();
        }

        //ATTEMPT
        using (var context = new BookDbContext(options))
        {
            var book = context.Books
                .Include(x => x.Reviews) //FIX 1
                .OrderBy(x => x.BookId).Last();
            book.AddReview(5, "great!", "me");
            context.SaveChanges(); //FIX 2
        }

        //VERIFY
        using (var context = new BookDbContext(options))
        {
            context.Books
                .Include(x => x.Reviews) //FIX 3
                .OrderBy(x => x.BookId).Last()
                .Reviews.Count.ShouldEqual(3);
        }
    }

    [Fact]
    public void FixTheProblemsUsingChangeTrackerClear()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();
        using var context = new BookDbContext(options);
        context.Database.EnsureClean();
        context.SeedDatabaseFourBooks();

        context.ChangeTracker.Clear(); //!!!!!!!!!!!!!!!!!!!!!!!

        //ATTEMPT
        var book = context.Books
            .Include(x => x.Reviews) //FIX 1
            .OrderBy(x => x.BookId).Last();
        book.AddReview(5, "great!", "me");
        context.SaveChanges(); //FIX 2

        //VERIFY
        context.ChangeTracker.Clear(); //!!!!!!!!!!!!!!!!!!!!!!!!
        context.Books
            .Include(x => x.Reviews) //FIX 3
            .OrderBy(x => x.BookId).Last()
            .Reviews.Count.ShouldEqual(3);
    }
}