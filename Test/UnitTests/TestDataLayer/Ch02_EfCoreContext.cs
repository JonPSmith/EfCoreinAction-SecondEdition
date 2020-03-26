// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.DataLayer
{
    public class Ch02_EfCoreContext
    {
        public Ch02_EfCoreContext(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public void TestCreateTestDataOk()
        {
            //SETUP

            //ATTEMPT
            var books = EfTestData.CreateFourBooks();

            //VERIFY
            books.Count.ShouldEqual(4);
            books.ForEach(x => x.AuthorsLink.Count.ShouldEqual(1));
            books[3].Reviews.Count.ShouldEqual(2);
            books[3].Promotion.ShouldNotBeNull();
        }

        /// <summary>
        ///     Thsi was written to see if the let statement in standard LINQ has a positive affect on the SQL command
        ///     The answer is - it doesn't, i.e. the SQL produced has two SELECT COUNT(*)... statements, not one
        /// </summary>
        [Fact]
        public void TestStandardLinqLetOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (!showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var books = (from book in context.Books
                        let count = book.Reviews.Count
                        select new {Count1 = count, Count2 = count}
                    ).ToList();

                //VERIFY
                books.First().Count1.ShouldEqual(books.First().Count2);
            }
        }

        [Fact]
        public void TestWriteTestDataSqliteOk()
        {
            //SETUP
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var books = EfTestData.CreateFourBooks();
                context.Books.AddRange(books);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(4);
                context.Books.Count(p => p.Title.StartsWith("Quantum")).ShouldEqual(1);
            }
        }
    }
}