// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.AdminServices.Concrete;
using ServiceLayer.CheckoutServices.Concrete;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch03_ChangePriceOfferService
    {
        [Theory]
        [InlineData(0, false)]
        [InlineData(3, true)]
        public void TestGetOriginal(int bookIndex, bool hasPromotion)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var books = context.SeedDatabaseFourBooks();

                var service = new ChangePriceOfferService(context);

                //ATTEMPT
                var priceOffer = service.GetOriginal(books[bookIndex].BookId);

                //VERIFY
                (priceOffer.NewPrice != books[bookIndex].Price).ShouldEqual(hasPromotion);
            }
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(3, 0)]
        public void AddRemovePriceOffer(int bookIndex, int numPriceOffers)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var books = context.SeedDatabaseFourBooks();

                var service = new ChangePriceOfferService(context);
                var priceOffer = service.GetOriginal(books[bookIndex].BookId);

                //ATTEMPT
                var error = service.AddRemovePriceOffer(priceOffer);

                //VERIFY
                context.PriceOffers.Count().ShouldEqual(numPriceOffers);
            }
        }
    }
}