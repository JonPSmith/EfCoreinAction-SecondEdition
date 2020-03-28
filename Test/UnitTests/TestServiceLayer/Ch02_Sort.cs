// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch02_Sort
    {
        [Fact]
        public void CheckSortVotes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var sorted = context.Books.MapBookToDto().OrderBooksBy(OrderByOptions.ByVotes).ToList();

                //VERIFY
                sorted.First().Title.ShouldEqual("Quantum Networking");
            }
        }

        [Fact]
        public void CheckSortPriceBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var ex = Assert.Throws<NotSupportedException>(() =>
                    context.Books.MapBookToDto().OrderBooksBy(OrderByOptions.ByPriceLowestFirst).ToList());

                //VERIFY
                ex.Message.ShouldStartWith("SQLite cannot order by expressions of type 'decimal'.");
            }
        }
    }
}