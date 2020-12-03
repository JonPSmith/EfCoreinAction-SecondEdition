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
using Test.Chapter16Listings;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestDirectQueryCosmosDb : IDisposable
    {
        private ITestOutputHelper _output;
        private readonly CosmosDbContext _cosmosContext;
        private readonly Container _cosmosContainer;
        private readonly BookDbContext _sqlContext;

        public TestDirectQueryCosmosDb(ITestOutputHelper output)
        {
            _output = output;
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

        [RunnableInDebugOnly]
        public async Task TestWritePerformanceOk()
        {
            //SETUP
            await _cosmosContext.Database.EnsureDeletedAsync();
            await _cosmosContext.Database.EnsureCreatedAsync();
            await _sqlContext.Database.EnsureCreatedAsync();

            int i = 1;
            i = await WriteToCosmosThreeWays(i);
            i = await WriteToCosmosThreeWays(i);
            await WriteToCosmosThreeWays(i);

            //VERIFY

        }

        private async Task<int> WriteToCosmosThreeWays(int i)
        {
            int numToAdd = 100;

            var directBooks = BookTestData.CreateDummyBooks(numToAdd).ToList()
                .Select(x => new DirectCosmosBook(i++, x)).ToList();

            //ATTEMPT
            using (new TimeThings(_output, "Direct", numToAdd))
            {
                foreach (var book in directBooks)
                {
                    var response = await _cosmosContainer.CreateItemAsync(book);
                }
            }

            using (new TimeThings(_output, "SQL", numToAdd))
            {
                _sqlContext.AddRange(BookTestData.CreateDummyBooks(numToAdd));
                await _sqlContext.SaveChangesAsync();
            }

            var cBooks = BookTestData.CreateDummyBooks(numToAdd).AsQueryable()
                .MapBookToCosmosBook().ToList();
            cBooks.ForEach(x => x.BookId = i++);
            using (new TimeThings(_output, "Via EfCore", numToAdd))
            {
                _cosmosContext.AddRange(cBooks);
                await _cosmosContext.SaveChangesAsync();
            }

            return i;
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