// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.CosmosDb.Services;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.Azure.Cosmos;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestDirectQueryCosmosDb : IDisposable
    {

        private readonly CosmosDbContext _cosmosContext;
        private readonly Container _cosmosContainer;
        private readonly BookDbContext _sqlContext;

        public TestDirectQueryCosmosDb()
        {
            var tuple = this.GetCosmosContextAndContainer();
             _cosmosContext = tuple.cosmosContext;
             _cosmosContainer = tuple.Container;

           var sqlOptions = SqliteInMemory.CreateOptions<BookDbContext>();
            _sqlContext = new BookDbContext(sqlOptions);


        }

        public void Dispose()
        {
            _cosmosContext?.Dispose();
            _sqlContext?.Dispose();
        }

        private async Task ResetDatabasesAndSeedAsync()
        {
            await _cosmosContext.Database.EnsureDeletedAsync();
            await _cosmosContext.Database.EnsureCreatedAsync();
            await _sqlContext.Database.EnsureCreatedAsync();

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);
            var sqlBooks = _sqlContext.SeedDatabaseFourBooks();
            foreach (var sqlBook in sqlBooks)
            {
                await service.AddCosmosBookAsync(sqlBook.BookId);
            }
        }


        [Fact]
        public async Task TestDirectCountOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<int>(new QueryDefinition("SELECT VALUE Count(c) FROM c"));
            var count = (await resultSet.ReadNextAsync()).First();

            //VERIFY
            count.ShouldEqual(4);
        }

        [Fact]
        public async Task TestDirectBooksOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<CosmosBook>(new QueryDefinition("SELECT * FROM c"));
            var books = (await resultSet.ReadNextAsync()).ToList();

            //VERIFY
            books.Count.ShouldEqual(4);
        }

        [Fact]
        public async Task TestDirectBooksOrderByOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<CosmosBook>(new QueryDefinition("SELECT * FROM c ORDER BY c.BookId DESC"));
            var books = (await resultSet.ReadNextAsync()).ToList();

            //VERIFY
            books.Count.ShouldEqual(4);
            books.Select(x => x.BookId).ShouldEqual(new []{4,3,2,1});
        }

        [Fact]
        public async Task TestDirectBooksPagingOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<CosmosBook>(new QueryDefinition("SELECT * FROM c OFFSET 2 LIMIT 1"));
            var books = (await resultSet.ReadNextAsync()).ToList();

            //VERIFY
            books.Count.ShouldEqual(1);
            books.Select(x => x.BookId).ShouldEqual(new[] { 3 });
        }

        [Fact]
        public async Task TestGetTagsOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<string>(
                //NOTE if query contains a subquery then you must define every property to load
                new QueryDefinition("SELECT DISTINCT value f.TagId FROM c JOIN f in c.Tags"));
            var tags = (await resultSet.ReadNextAsync()).OrderBy(x => x).ToList();

            //VERIFY
            tags.ShouldEqual(new List<string>{ "Architecture", "Editor's Choice", "Quantum Entanglement", "Refactoring" });
        }


        [Fact]
        public async Task TestFilterByTagsOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<CosmosBook>(
                //NOTE if query contains a subquery then you must define every property to load
                new QueryDefinition("SELECT c.Title, c.TagsString FROM c JOIN f in c.Tags WHERE f.TagId = 'Architecture'"));
            var books = (await resultSet.ReadNextAsync()).ToList();

            //VERIFY
            books.Count.ShouldEqual(2);
        }

        [Fact]
        public async Task TestBuildListOfYearsDropdownOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var resultSet = _cosmosContainer.GetItemQueryIterator<int>(
                //NOTE if query contains a subquery then you must define every property to load
                new QueryDefinition($"SELECT DISTINCT VALUE c.YearPublished FROM c WHERE c.YearPublished > {2000}"));
            var years = (await resultSet.ReadNextAsync()).ToList();

            //VERIFY
        }

    }
}