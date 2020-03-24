// // Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// // Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using ServiceLayer.DatabaseServices.Concrete;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace test.UnitTests.DataLayer
{
    public class Ch02_StringSearch
    {
        public Ch02_StringSearch(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Theory]
        [InlineData("Book0001 Title", 1)]
        [InlineData("book0001 TITLE", 0)]
        public void TestEqualsExact(string searchString, int expectedCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title == searchString)
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(expectedCount);
            }
        }

        [Theory]
        [InlineData("Book0001 Title", 1)]
        [InlineData("book0001 TITLE", 1)]
        public void TestLikeExact(string searchString, int expectedCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => EF.Functions.Like(p.Title, searchString))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(expectedCount);
            }
        }

        [Fact]
        public async Task FindBooksWithCSharpInTheirTitleContains()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var appWwwrootDir =
                    Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir(), @"../BookApp/wwwroot/"));
                await context.SeedDatabaseIfNoBooksAsync(appWwwrootDir);

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.Contains("C#"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(5);
            }
        }

        [Fact]
        public async Task FindBooksWithCSharpInTheirTitleLike()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var appWwwrootDir =
                    Path.GetFullPath(Path.Combine(TestData.GetCallingAssemblyTopLevelDir(), @"../BookApp/wwwroot/"));
                await context.SeedDatabaseIfNoBooksAsync(appWwwrootDir);

                //ATTEMPT
                var bookTitles = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "%C#%"))
                    .Select(p => p.Title)
                    .ToList();


                //VERIFY
                bookTitles.Count.ShouldEqual(5);
                foreach (var title in bookTitles)
                {
                    _output.WriteLine(title);
                }
            }
        }

        [Fact]
        public void TestEndsWith()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.EndsWith("1 Title"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestLike()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "Book00_5%"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestLikeLowerCase()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => EF.Functions.Like(p.Title, "book00_5%"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(4);
            }
        }

        [Fact]
        public void TestStartsWith()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(40);

                //ATTEMPT
                var books = context.Books
                    .Where(p => p.Title.StartsWith("Book001"))
                    .ToList();

                //VERIFY
                books.Count.ShouldEqual(10);
            }
        }
    }
}