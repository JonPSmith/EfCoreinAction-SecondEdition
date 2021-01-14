// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.CachedValues.EventHandlers;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using GenericEventRunner.ForSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestBookGeneratorSqlOnly
    {
        private readonly ITestOutputHelper _output;

        public TestBookGeneratorSqlOnly(ITestOutputHelper output)
        {
            _output = output;
        }

        private IServiceProvider BuildServiceProvider(DbContextOptions<BookDbContext> sqlOptions)
        {
            var services = new ServiceCollection();
            services.AddSingleton(sqlOptions);

            return services.BuildServiceProvider();
        }


        [Fact]
        public void TestBookCreateTestBookOk()
        {
            //SETUP

            //ATTEMPT
            var book = new Book("book title", new DateTime(2000,1,2), false, 
                "publisher", 123, "imageurl", 
                new List<Author>{ new Author("Author1", null)},
                new List<Tag>{ new Tag("Tag1")},
                new List<byte>{1,2,3}, "reviewUser" );

            //VERIFY
            book.ToString().ShouldEqual($"book title: by Author1. Price 123, 3 reviews. Published {new DateTime(2000, 1, 2):d} Tags: Tag1");
            book.ReviewsAverageVotes.ShouldEqual(2);
            book.ReviewsCount.ShouldEqual(3);
        }

        [Fact]
        public void TestLoadBooksFromTestDataOk()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir(), "seedData\\");
            var loader = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //ATTEMPT
            var loadedBooks = loader.LoadBooks(true).ToList();

            //VERIFY
            loadedBooks.Count().ShouldEqual(6);
            loadedBooks.Count(x => x.Details?.Description != null).ShouldEqual(0);
            loadedBooks.All(x => x.Tags.Select(x => x.TagId).Contains("Manning books")).ShouldBeTrue();
        }

        [Fact]
        public async Task TestWriteBooksAsyncNoDataCausesNewDbOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureClean();

            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var serviceProvider = BuildServiceProvider(options);
            var generator = new BookGenerator(serviceProvider);

            //ATTEMPT
            await generator.WriteBooksAsync(fileDir, false, 1, true, default);

            //VERIFY
            context.Books.Count().ShouldEqual(6);
            context.Authors.Count().ShouldEqual(8);
            context.Tags.Count().ShouldEqual(6);
            context.Books
                .Include(x => x.Tags)
                .Count(x => x.Tags.Select(y => y.TagId).Contains("Manning books"))
                .ShouldEqual(6);
        }

        [Fact]
        public async Task TestWriteBooksAsyncWipeDatabaseOk()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = this.CreateUniqueClassOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            await context.SeedDatabaseWithBooksAsync(fileDir);

            context.ChangeTracker.Clear();

            var serviceProvider = BuildServiceProvider(options);
            var generator = new BookGenerator(serviceProvider);

            //ATTEMPT
            await generator.WriteBooksAsync(fileDir, true, 10, true, default);

            //VERIFY
            context.Books.Count().ShouldEqual(10);
            context.Books
                .Include(x => x.Tags)
                .Count(x => x.Tags.Select(y => y.TagId).Contains("Manning books"))
                .ShouldEqual(6);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public async Task TestWriteBooksAsyncAskNumberOk(int totalBooks)
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            options.TurnOffDispose();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            await context.SeedDatabaseWithBooksAsync(fileDir);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var serviceProvider = BuildServiceProvider(options);
            var generator = new BookGenerator(serviceProvider);

            await generator.WriteBooksAsync(fileDir, false, totalBooks, true, default);

            //VERIFY
            context.Books.Count().ShouldEqual(totalBooks);
            options.ManualDispose();
        }

        [Fact]
        public async Task TestWriteBooksAsyncCheckReviews()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            options.TurnOffDispose();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            await context.SeedDatabaseWithBooksAsync(fileDir);

            context.ChangeTracker.Clear();

            //ATTEMPT
            var serviceProvider = BuildServiceProvider(options);
            var generator = new BookGenerator(serviceProvider);

            await generator.WriteBooksAsync(fileDir, false, 20, true, default);

            //VERIFY
            foreach (var book in context.Books)
            {
                _output.WriteLine(book.ToString());
            }

            var books = context.Books.Include(x => x.Reviews).ToList();
            books.Count(x => x.Reviews.Count > 0).ShouldEqual(13);
            //Can't get exact review value compare
            books.All(x => x.ReviewsCount == x.Reviews.Count).ShouldBeTrue();
            books.Where(x => x.ReviewsCount > 0)
                .All(x => x.ReviewsAverageVotes == x.Reviews.Average(y => y.NumStars)).ShouldBeTrue();
            options.ManualDispose();
        }

        [Fact]
        public async Task TestWriteBooksAsyncCheckAddUpdateDates()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            options.TurnOffDispose();
            var timeNow = DateTime.UtcNow;

            var eventConfig = new GenericEventRunnerConfig
            {
                NotUsingDuringSaveHandlers = true
            };
            eventConfig.AddActionToRunAfterDetectChanges<BookDbContext>(localContext =>
                localContext.ChangeChecker());
            using var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>(null, eventConfig);
            context.Database.EnsureCreated();
            await context.SeedDatabaseWithBooksAsync(fileDir); //The LastUpdatedUtc is set via events

            context.ChangeTracker.Clear();

            //ATTEMPT
            var serviceProvider = BuildServiceProvider(options);

            var generator = new BookGenerator(serviceProvider);
            await generator.WriteBooksAsync(fileDir, false, 20, true, default);

            //VERIFY
            var books = context.Books
                .Include(x => x.Reviews)
                .Include(x => x.AuthorsLink).ToList();
            books.All(x => x.LastUpdatedUtc >= timeNow).ShouldBeTrue();
            books.SelectMany(x => x.Reviews).All(x => x.LastUpdatedUtc >= timeNow).ShouldBeTrue();
            books.SelectMany(x => x.AuthorsLink).All(x => x.LastUpdatedUtc >= timeNow).ShouldBeTrue();
            options.ManualDispose();
        }

    }
}