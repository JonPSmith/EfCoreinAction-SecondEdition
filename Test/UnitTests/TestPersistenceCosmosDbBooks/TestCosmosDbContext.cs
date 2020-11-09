// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestCosmosDbContext
    {
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

        [Fact] public async Task TestFilterCosmosBookByTagsEfCoreBad()
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
    }
}