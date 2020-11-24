// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.CosmosDb.Services;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.CosmosDirect.Books.Services;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceCosmosDbBooks
{
    public class TestCosmosDirectRead : IDisposable
    {

        private readonly CosmosDbContext _cosmosContext;
        private readonly string _databaseName;
        private readonly BookDbContext _sqlContext;

        private SortFilterPageOptions _options;

        public TestCosmosDirectRead()
        {
            var tuple = this.GetCosmosDbAndDatabaseName();
            _cosmosContext = tuple.context;
            _databaseName = tuple.databaseName;

            var sqlOptions = SqliteInMemory.CreateOptions<BookDbContext>();
            _sqlContext = new BookDbContext(sqlOptions);

            _options = new SortFilterPageOptions
            {
                PageSize = 10,
                PageNum = 0
            };

            //need to do this to to setup PrevCheckState 
            _options.SetupRestOfDto(4);
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
        public async Task TestCosmosDirectCountOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var count = await _cosmosContext.CosmosDirectCountAsync(_options, _databaseName);

            //VERIFY
            count.ShouldEqual(4);
        }

        [Fact]
        public async Task TestCosmosDirectReadOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var books = await _cosmosContext.CosmosDirectQueryAsync(_options, _databaseName);

            //VERIFY
            books.Count().ShouldEqual(4);
        }

        [Fact]
        public async Task TestGetFilterDropDownValuesAsyncPublishedDatesOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var dropdowns = await _cosmosContext.GetFilterDropDownValuesAsync(BooksFilterBy.ByPublicationYear, _databaseName);

            //VERIFY
            dropdowns.Select(x => x.Value).ShouldEqual(new []{ "Coming Soon", "2003", "2002", "1999" });
        }

        [Fact]
        public async Task TestGetFilterDropDownValuesAsyncTagsOk()
        {
            //SETUP
            await ResetDatabasesAndSeedAsync();

            //ATTEMPT
            var dropdowns = await _cosmosContext.GetFilterDropDownValuesAsync(BooksFilterBy.ByTags, _databaseName);

            //VERIFY
            dropdowns.Select(x => x.Value).ShouldEqual(new[] { "Architecture", "Editor's Choice", "Quantum Entanglement", "Refactoring" });
        }
    }
}