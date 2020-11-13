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
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureCosmosDb
{
    public class TestBookToCosmosBookService : IDisposable
    {

        private readonly CosmosDbContext _cosmosContext;
        private readonly BookDbContext _sqlContext;

        public TestBookToCosmosBookService()
        {
            var cosmosOptions = this.GetCosmosDbOptions<CosmosDbContext>();
             _cosmosContext = new CosmosDbContext(cosmosOptions);
             
            var sqlOptions = SqliteInMemory.CreateOptions<BookDbContext>();
            _sqlContext = new BookDbContext(sqlOptions);
        }

        public void Dispose()
        {
            _cosmosContext?.Dispose();
            _sqlContext?.Dispose();
        }

        private async Task<List<Book>> ResetDatabasesAndSeedAsync()
        {
            await _cosmosContext.Database.EnsureDeletedAsync();
            await _cosmosContext.Database.EnsureCreatedAsync();
            await _sqlContext.Database.EnsureCreatedAsync();

            return _sqlContext.SeedDatabaseFourBooks();
        }

        private async Task AddDummyCosmosBook(int bookId)
        {
            var dummyCosmosBook = new CosmosBook { BookId = bookId, Title = "dummy" };
            _cosmosContext.Add(dummyCosmosBook);
            await _cosmosContext.SaveChangesAsync();
            _cosmosContext.ChangeTracker.Clear();
        }

        [Fact]
        public async Task TestAddCosmosBookOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.AddCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            var cBook = _cosmosContext.Books.Single(x => x.BookId == seeded[3].BookId);
            cBook.ToString().ShouldEqual("Quantum Networking: by Future Person. Price 219.0, 2 reviews. Published 01/01/2057, Tags: | Quantum Entanglement |");
        }

        [Fact]
        public async Task TestAddCosmosBookNoSqlBookOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();
            _sqlContext.Remove(seeded[3]);
            await _sqlContext.SaveChangesAsync();

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.AddCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            (await _cosmosContext.Books.CountAsync()).ShouldEqual(0);
        }

        [Fact]
        public async Task TestUpdateCosmosBookOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();
            await AddDummyCosmosBook(seeded[3].BookId);

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.UpdateCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            var cBook = await _cosmosContext.Books.SingleAsync(x => x.BookId == seeded[3].BookId);
            cBook.ToString().ShouldEqual("Quantum Networking: by Future Person. Price 219.0, 2 reviews. Published 01/01/2057, Tags: | Quantum Entanglement |");
        }

        [Fact]
        public async Task TestUpdateCosmosBookNotThereTurnedIntoAdd()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.UpdateCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            var cBook = await _cosmosContext.Books.SingleAsync(x => x.BookId == seeded[3].BookId);
            cBook.ToString().ShouldEqual("Quantum Networking: by Future Person. Price 219.0, 2 reviews. Published 01/01/2057, Tags: | Quantum Entanglement |");
        }

        [Fact]
        public async Task TestDeleteCosmosBookOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();
            await AddDummyCosmosBook(seeded[3].BookId);

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.DeleteCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            var cBook = await _cosmosContext.Books.SingleOrDefaultAsync(x => x.BookId == seeded[3].BookId);
            cBook.ShouldBeNull();
        }

        [Fact]
        public async Task TestDeleteCosmosBookNotThereOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();
            await AddDummyCosmosBook(seeded[2].BookId);

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.DeleteCosmosBookAsync(seeded[3].BookId);

            //VERIFY
            var cBook = await _cosmosContext.Books.SingleOrDefaultAsync(x => x.BookId == seeded[3].BookId);
            cBook.ShouldBeNull();
            (await _cosmosContext.Books.CountAsync()).ShouldEqual(1);
        }

        [Fact]
        public async Task TestUpdateManyCosmosBookOk()
        {
            //SETUP
            var seeded = await ResetDatabasesAndSeedAsync();
            await AddDummyCosmosBook(seeded[0].BookId);
            await AddDummyCosmosBook(seeded[1].BookId);

            var service = new BookToCosmosBookService(_sqlContext, _cosmosContext);

            //ATTEMPT
            await service.UpdateManyCosmosBookAsync(new List<int>{ seeded[0].BookId, seeded[1].BookId });

            //VERIFY
            var cBooks = await _cosmosContext.Books.ToListAsync();
            cBooks.Count.ShouldEqual(2);
            cBooks.All(x => x.AuthorsOrdered == "Martin Fowler").ShouldBeTrue();
        }
    }
}