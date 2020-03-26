// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BizDbAccess.Orders;
using DataLayer.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBizDbAccess
{
    public class Ch04_PlaceOrderDbAccess
    {
        [Fact]
        public void TestCheckoutListTwoBooksSqLite()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var dbAccess = new PlaceOrderDbAccess(context);

                //ATTEMPT
                var booksDict = dbAccess.FindBooksByIdsWithPriceOffers(new []{1, 4});

                //VERIFY
                booksDict.Count.ShouldEqual(2);
                booksDict[1].Promotion.ShouldBeNull();
                booksDict[4].Promotion.ShouldNotBeNull();
            }
        }
    }
}