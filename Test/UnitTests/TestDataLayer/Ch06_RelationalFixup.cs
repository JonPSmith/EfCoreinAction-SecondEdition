// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_RelationalFixup
    {
        private readonly ITestOutputHelper _output;

        public Ch06_RelationalFixup(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestReadBookWithInclude()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                bookId = context.Books.Single(x => x.Reviews.Any()).BookId;
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var bookWithReviews  = context.Books
                    .Include(x => x.Reviews)
                    .Single(x => x.BookId == bookId);

                //VERIFY
                bookWithReviews.Reviews.Count.ShouldEqual(2);
            }
        }

        [Fact]
        public void TestReadBookWithSecondRead()
        {
            //SETUP
            int bookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                bookId = context.Books.Single(x => x.Reviews.Any()).BookId;
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var bookWithReviews = context.Books
                    .Single(x => x.BookId == bookId);
                bookWithReviews.Reviews.ShouldBeNull();
                var reviews = context.Set<Review>()
                    .Where(x => x.BookId == bookId)
                    .ToList();

                //VERIFY
                bookWithReviews.Reviews.Count.ShouldEqual(2);
            }
        }


    }
}