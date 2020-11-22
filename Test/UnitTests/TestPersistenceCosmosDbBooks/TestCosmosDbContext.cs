// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.ServiceLayer.CosmosEf.Books;
using BookApp.ServiceLayer.CosmosEf.Books.Services;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using BookApp.UI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Test.TestHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestCosmosDbContext
    {
        private ITestOutputHelper _output;

        public TestCosmosDbContext(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestAccessCosmosEmulator()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<CosmosDbContext>();
            using var context = new CosmosDbContext(options);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
        }

        [Fact]
        public void TestAccessCosmosEmulatorWithLogging()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var connectionString = config.GetConnectionString(CosmosSetupHelpers.CosmosConnectionName);
            var dbSettings = new CosmosDbSettings(connectionString, GetType().Name);
            var builder = new DbContextOptionsBuilder<CosmosDbContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName)
                .LogTo(_output.WriteLine, LogLevel.Information);
            using var context = new CosmosDbContext(builder.Options);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
        }

        [Fact]
        public async Task TestWriteCosmosBookWithTags()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<CosmosDbContext>();
            using var context = new CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //ATTEMPT
            var book = new CosmosBook
            {
                BookId = 123,
                Title = "Test",
                Tags = new List<CosmosTag> {new CosmosTag("Tag1"), new CosmosTag("Tag2")}
            };
            context.Add(book);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.Books.SingleAsync();
            readBook.BookId.ShouldEqual(123);
            readBook.Tags.Select(x => x.TagId).ShouldEqual(new [] { "Tag1", "Tag2" });
        }

        [Fact]
        public async Task TestFilterDropdownYears()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<CosmosDbContext>();
            using var context = new CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var book1 = new CosmosBook
            {
                BookId = 123,
                Title = "Year2000",
                PublishedOn = new DateTime(2000,1,1),
                YearPublished = 2000
            };
            var book2 = new CosmosBook
            {
                BookId = 567,
                Title = "Year3000",
                PublishedOn = new DateTime(3000, 1, 1),
                YearPublished = 3000
            };
            context.AddRange(book1, book2);
            await context.SaveChangesAsync();

            var service = new CosmosEfBookFilterDropdownService(context);

            //ATTEMPT
            var dropdown = await service.GetFilterDropDownValuesAsync(CosmosBooksFilterBy.ByPublicationYear);

            //VERIFY
            dropdown.Select(x => x.Text).ShouldEqual(new []{ "Coming Soon", "2000"});
        }

        [Fact]
        public async Task TestFilterCosmosBookByTagsEfCoreBad()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<CosmosDbContext>();
            using var context = new CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var book1 = new CosmosBook
            {
                BookId = 123,
                Title = "Test",
                Tags = new List<CosmosTag> { new CosmosTag("Tag1"), new CosmosTag("Tag2") }
            };
            var book2 = new CosmosBook
            {
                BookId = 567,
                Title = "Test",
                Tags = new List<CosmosTag> { new CosmosTag("Tag3"), new CosmosTag("Tag2") }
            };
            context.AddRange(book1, book2);
            await context.SaveChangesAsync();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await context.Books.Where(x => x.Tags
                    .Select(y => y.TagId).Contains("Tag3")).ToListAsync());

            //VERIFY
            ex.Message.ShouldContain("could not be translated.");
        }

        [Fact] public async Task TestFilterCosmosBookByTagsEfCoreViaTagsStringOk()
        {
            //SETUP
            var options = this.GetCosmosDbOptions<CosmosDbContext>();
            using var context = new CosmosDbContext(options);
            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            var book1 = new CosmosBook
            {
                BookId = 123,
                Title = "Test1",
                Tags = new List<CosmosTag> { new CosmosTag("Tag1"), new CosmosTag("Tag2") },
                TagsString = "| Tag1 | Tag2 |"
            };
            var book2 = new CosmosBook
            {
                BookId = 567,
                Title = "Test2",
                Tags = new List<CosmosTag> { new CosmosTag("Tag3"), new CosmosTag("Tag2") },
                TagsString = "| Tag3 | Tag2 |"
            };
            context.AddRange(book1, book2);
            await context.SaveChangesAsync();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var books = await context.Books.Where(x => x.TagsString.Contains("| Tag3 |")).ToListAsync();

            //VERIFY
            books.Count.ShouldEqual(1);
            books.Single().Title.ShouldEqual("Test2");
        }
    }
}