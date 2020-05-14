// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BizDbAccess.Orders;
using DataLayer.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBizDbAccess
{
    public class Ch04_PlaceOrderDbAccess
    {
        private readonly ITestOutputHelper _output;

        public Ch04_PlaceOrderDbAccess(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestCheckoutListTwoBooksSqLite()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var dbAccess = new PlaceOrderDbAccess(context);

                //ATTEMPT
                showlog = true;
                var booksDict = dbAccess.FindBooksByIdsWithPriceOffers(new []{1, 4});

                //VERIFY
                booksDict.Count.ShouldEqual(2);
                booksDict[1].Promotion.ShouldBeNull();
                booksDict[4].Promotion.ShouldNotBeNull();
            }
        }
    }
}