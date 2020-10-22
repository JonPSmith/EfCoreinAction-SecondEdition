// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.Book.EventHandlers;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestBookGenerator
    {
        private ITestOutputHelper _output;

        public TestBookGenerator(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestLoadBooksFromTestDataOk()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir(), "seedData\\");

            //ATTEMPT
            var loadedBooks = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //VERIFY
            loadedBooks.Books.Count().ShouldEqual(6);
            loadedBooks.Books.Count(x => x.Details?.Description != null).ShouldEqual(0);
        }

        [Fact]
        public async Task TestWriteBooksAsyncNoDataCausesNewDbOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            using (var context = new BookDbContext(options))
            {
                var fileDir = Path.Combine(TestData.GetTestDataDir());
                var generator = new BookGenerator(context);

                //ATTEMPT
                await generator.WriteBooksAsync(fileDir, false, 1, true, default);

                //VERIFY
                context.Books.Count().ShouldEqual(6);
                context.Authors.Count().ShouldEqual(8);
                context.Tags.Count().ShouldEqual(5);
            }
        }

        [Fact]
        public async Task TestWriteBooksAsyncWipeDatabaseOk()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = this.CreateUniqueClassOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                await context.SeedDatabaseWithBooksAsync(fileDir);
            }
            using (var context = new BookDbContext(options))
            {
                var generator = new BookGenerator(context);

                //ATTEMPT
                await generator.WriteBooksAsync(fileDir, true, 10, true, default);

                //VERIFY
                context.Books.Count().ShouldEqual(10);
            }
        }

        [Theory]
        [InlineData(10)]
        [InlineData(20)]
        public async Task TestWriteBooksAsyncAskNumberOk(int totalBooks)
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                await context.SeedDatabaseWithBooksAsync(fileDir);
            }
            using (var context = new BookDbContext(options))
            {
                var generator = new BookGenerator(context);

                //ATTEMPT
                await generator.WriteBooksAsync(fileDir, false, totalBooks, true, default);

                //VERIFY
                context.Books.Count().ShouldEqual(totalBooks);
            }
        }

        [Fact]
        public async Task TestWriteBooksAsyncCheckReviews()
        {
            //SETUP
            var fileDir = Path.Combine(TestData.GetTestDataDir());
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                await context.SeedDatabaseWithBooksAsync(fileDir);
            }
            using (var context = options.CreateDbWithDiForHandlers<BookDbContext, ReviewAddedHandler>())
            {
                var generator = new BookGenerator(context);

                //ATTEMPT
                await generator.WriteBooksAsync(fileDir, false, 10, true, default);

                //VERIFY
                foreach (var book in context.Books)
                {
                    _output.WriteLine(book.ToString());
                }
                context.Books
                    .Count(x => x.Reviews.Count() > 0).ShouldEqual(4);
            }
        }

    }
}