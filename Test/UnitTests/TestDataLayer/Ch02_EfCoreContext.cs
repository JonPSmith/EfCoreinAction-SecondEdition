// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
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
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var query = from book in context.Books
                        let count = book.Reviews.Count
                        select new {Count1 = count, Count2 = count};
                var books = query.ToList();

                //VERIFY
                books.First().Count1.ShouldEqual(books.First().Count2);
                var sql = query.ToQueryString();
                Regex.Matches(sql, @"SELECT COUNT\(\*\)").Count.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestWriteTestDataSqliteInMemoryOk()
        {
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
                context.Books.Count(p => 
                    p.Title == "Refactoring").ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateDataSqliteInMemoryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log => _output.WriteLine(log.Message));
            using (var context = new EfCoreContext(options))
            {

                //ATTEMPT
                context.Database.EnsureCreated();

                //VERIFY

            }
        }

        [Fact]
        public async Task TestIncludeWithCountOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var bookId = context.SeedDatabaseFourBooks().Last().BookId;
                
                context.ChangeTracker.Clear();

                //ATTEMPT
                var query1 = context.Books.Include(b => b.Reviews)
                    .Where(b => b.BookId == bookId)
                    .Select(b => b.Reviews.Count);
                var reviewCount1 = await query1.SingleAsync();
                var query2 = context.Books
                    .Where(b => b.BookId == bookId)
                    .Select(b => b.Reviews.Count);
                var reviewCount2 = await query2.SingleAsync();

                //VERIFY
                _output.WriteLine(query1.ToQueryString());
                reviewCount1.ShouldEqual(2);
                _output.WriteLine(query2.ToQueryString());
                reviewCount2.ShouldEqual(2);
            }
        }
    }
}