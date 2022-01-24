// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.CommunityStandUp;

public class ExampleUnitTest
{
    [Fact]
    public void TestExampleCommunityStandup()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();

        using var context = new BookDbContext(options);

        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        context.SeedDatabaseFourBooks();

        context.ChangeTracker.Clear();

        //ATTEMPT
        var testDate = new DateTime(2020, 1, 1);
        var books = context.Books
            .Where(x => x.PublishedOn < testDate)
            .ToList();

        //VERIFY
        books.Count.ShouldEqual(3);
        books.Select(x => x.Title).ShouldEqual(
            new[] { "Refactoring", "Patterns of Enterprise Application Architecture", "Domain-Driven Design" });
    }
}