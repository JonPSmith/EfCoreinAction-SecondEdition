// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
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
    public class TestManningBookLoad
    {
        private readonly ITestOutputHelper _output;

        public TestManningBookLoad(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLoadBooksOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.Combine(TestData.GetTestDataDir(), "seedData\\");
            var loader = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //ATTEMPT
            var loadedBooks = loader.LoadBooks(false);

            //VERIFY
            loadedBooks.Count().ShouldEqual(6);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void TestLoadBooksCheckTagsOk(bool tagAsOriginal)
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.Combine(TestData.GetTestDataDir(), "seedData\\");
            var loader = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //ATTEMPT
            var loadedBooks = loader.LoadBooks(tagAsOriginal);

            //VERIFY
            foreach (var book in loadedBooks)
            {
                book.Tags.Any().ShouldBeTrue();
                book.Tags.Select(x => x.TagId).Contains("Manning books").ShouldEqual(tagAsOriginal);
            }
        }

        [Fact]
        public void TestManuallyAddDetailsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var book = BookTestData.CreateDummyBookOneAuthor();
            book.SetBookDetails("d","aa", "ar", "at", "w");
            context.Add(book);
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(1);
        }

        [Fact]
        public void TestLoadBooksAddTenToDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.Combine(TestData.GetTestDataDir(), "seedData\\");

            var loader = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //ATTEMPT
            context.AddRange(loader.LoadBooks(false));
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(6);
            context.Authors.Count().ShouldEqual(8);
            context.Tags.Count().ShouldEqual(5);

        }


    }
}