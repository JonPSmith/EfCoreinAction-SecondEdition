// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.CommunityStandUp;

public class WaysToGetUniqueDatabaseName
{
    [Fact]
    public void TestManuallyCreateUniqueDatabaseName()
    {
        //SETUP
        var connectionString
            = $"Server=(localdb)\\mssqllocaldb;Database={GetType().Name};Trusted_Connection=True";
        var builder = new DbContextOptionsBuilder<BookDbContext>()
            .UseSqlServer(connectionString);

        //The following method in the EfCore.TestSupport encapsulates the code above
        //see https://github.com/JonPSmith/EfCore.TestSupport/wiki/Using-SQL-Server-databases
        //var options = this.CreateUniqueClassOptions<BookDbContext>();

        using var context = new BookDbContext(builder.Options);

        context.Database.EnsureClean();
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

    [Fact]
    public void TestUsingTestSupportLibrary()
    {
        //SETUP
        var options = this.CreateUniqueClassOptions<BookDbContext>();
        using var context = new BookDbContext(options);

        context.Database.EnsureClean();
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



    [Fact]
    public void TestUseSqlLiteInMemoryDatabase()
    {
        //SETUP
        var connectionStringBuilder =
            new SqliteConnectionStringBuilder
                { DataSource = ":memory:" };
        var connectionString = connectionStringBuilder.ToString();
        var connection = new SqliteConnection(connectionString);
        connection.Open();
        var builder = new DbContextOptionsBuilder<BookDbContext>();
        builder.UseSqlite(connection);
        var options = builder.Options;

        //The following method in the EfCore.TestSupport encapsulates the code above
        //see https://github.com/JonPSmith/EfCore.TestSupport/wiki/Using-SQLite-in-memory-databases
        //var options = SqliteInMemory.CreateOptions<BookDbContext>();

        using var context = new BookDbContext(options);

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