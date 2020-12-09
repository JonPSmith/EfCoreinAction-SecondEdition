// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.CheckoutServices.Concrete;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch04_CheckoutListService
    {
        [Fact]
        public void TestCheckoutListBookWithPromotion()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //I select the last book, which has a promotion
            var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{Guid.NewGuid()},4,1");

            //ATTEMPT

            var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
            var list = service.GetCheckoutList();

            //VERIFY
            list.Count.ShouldEqual(1);
            list.First().BookPrice.ShouldEqual(219);
        }

        [Fact]
        public void TestCheckoutListOneBookDatabaseOneBook()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseDummyBooks(1);

            context.ChangeTracker.Clear();

            //two line items: BookId:1 NumBooks:1
            var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{Guid.NewGuid()},1,1");

            //ATTEMPT

            var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
            var list = service.GetCheckoutList();
                
            //VERIFY
            list.Count.ShouldEqual(1);
            list.First().BookId.ShouldEqual(1);
            list.First().BookPrice.ShouldEqual(1);
        }

        [Fact]
        public void TestCheckoutListTwoBooksSqLite()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseDummyBooks(10);

            context.ChangeTracker.Clear();

            //two line items: BookId:1 NumBooks:2, BookId:2 NumBooks:3
            var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{Guid.NewGuid()},1,2,2,3");

            //ATTEMPT

            var service = new CheckoutListService(context, mockCookieRequests.CookiesIn);
            var list = service.GetCheckoutList();

            //VERIFY
            for (int i = 0; i < list.Count(); i++)
            {
                list[i].BookId.ShouldEqual(i + 1);
                list[i].NumBooks.ShouldEqual((short)(i + 2));
                list[i].BookPrice.ShouldEqual((i + 1));
            }
        }
    }
}