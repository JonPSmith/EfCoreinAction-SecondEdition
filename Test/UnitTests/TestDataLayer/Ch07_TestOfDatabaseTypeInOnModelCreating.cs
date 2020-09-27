// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BookServices.QueryObjects;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch07_TestOfDatabaseTypeInOnModelCreating
    {
        private readonly ITestOutputHelper _output;

        public Ch07_TestOfDatabaseTypeInOnModelCreating(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestIsSqliteWorksInOnModelCreating()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var costs = context.Books.MapBookToDto()
                    .OrderBooksBy(OrderByOptions.ByPriceLowestFirst)
                    .Select(x => x.ActualPrice)
                    .ToArray();

                //VERIFY
                costs.ShouldEqual(new decimal[]{ 40, 53, 56, 219 });
            }
        }

    }
}