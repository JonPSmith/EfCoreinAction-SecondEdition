// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.CosmosDb.Books;
using Microsoft.EntityFrameworkCore;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestCosmosEmulator
    {
        private readonly ITestOutputHelper _output;

        public TestCosmosEmulator(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void AccessCosmosEmulatorViaConnectionString()
        {
            //SETUP
            var connectionString = //#A
                "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";
            var builder = new                                  //#B
                    DbContextOptionsBuilder<CosmosDbContext>() //#B
                .UseCosmos(                       //#C
                    connectionString,             //#D
                    "MyCosmosDatabase"); //#E
            using var context = new CosmosDbContext(builder.Options); //#F

            //ATTEMPT

            //VERIFY
        }
        /********************************************************************
        #A This is the connection string taken from the quickstart page of the emulator' web site
        #B We build the options for the CosmosDbContext
        #C UseCosmos method is found in the Microsoft.EntityFrameworkCore.Cosmos NuGet package
        #D The connection string if provided first
        #E Then the name you want for the database
        #F then you can create an instance of the application's DbContext
         *********************************************************************/

        [RunnableInDebugOnly]
        public async Task TestAccessCosmosEmulator()
        {
            //SETUP
            var options = this.CreateUniqueClassCosmosDbEmulator
                <CosmosDbContext>();
            using var context = new CosmosDbContext(options);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //ATTEMPT
            var book = new CosmosBook
            {
                BookId = 123,
                Title = "Test",
                Tags = new List<CosmosTag> { new CosmosTag("Tag1"), new CosmosTag("Tag2") }
            };
            context.Add(book);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.Books.SingleAsync();
            readBook.BookId.ShouldEqual(123);
            readBook.Tags.Select(x => x.TagId).ShouldEqual(new[] { "Tag1", "Tag2" });
        }

        [RunnableInDebugOnly]
        public async Task TestAccessCosmosEmulatorUniqueToMethod()
        {
            //SETUP
            var options = this.CreateUniqueMethodCosmosDbEmulator
                <CosmosDbContext>();
            using var context = new CosmosDbContext(options);

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();

            //ATTEMPT
            var book = new CosmosBook
            {
                BookId = 123,
                Title = "Test",
                Tags = new List<CosmosTag> { new CosmosTag("Tag1"), new CosmosTag("Tag2") }
            };
            context.Add(book);
            await context.SaveChangesAsync();

            //VERIFY
            context.ChangeTracker.Clear();
            var readBook = await context.Books.SingleAsync();
            readBook.BookId.ShouldEqual(123);
            readBook.Tags.Select(x => x.TagId).ShouldEqual(new[] { "Tag1", "Tag2" });
        }


    }
}