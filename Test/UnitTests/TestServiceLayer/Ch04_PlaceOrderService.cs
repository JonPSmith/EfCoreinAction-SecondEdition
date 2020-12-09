// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.CheckoutServices.Concrete;
using ServiceLayer.OrderServices.Concrete;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayer
{
    public class Ch04_PlaceOrderService
    {
        [Fact]
        public void TestPlaceOrderBad()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{Guid.NewGuid()},1,2");
            var service = new PlaceOrderService(mockCookieRequests.CookiesIn, mockCookieRequests.CookiesOut, context);

            //ATTEMPT
            var orderId = service.PlaceOrder(false);
            context.SaveChanges();

            //VERIFY
            orderId.ShouldEqual(0);
            service.Errors.Count.ShouldEqual(1);
            service.Errors.First().ErrorMessage.ShouldEqual("You must accept the T&Cs to place an order.");
        }

        [Fact]
        public void TestPlaceOrderOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var mockCookieRequests = new MockHttpCookieAccess(BasketCookie.BasketCookieName, $"{userId},1,2");
            var service = new PlaceOrderService(mockCookieRequests.CookiesIn, mockCookieRequests.CookiesOut, context);

            //ATTEMPT
            var orderId = service.PlaceOrder(true);
            context.SaveChanges();

            //VERIFY
            orderId.ShouldNotEqual(0);
            service.Errors.Count.ShouldEqual(0);
            context.Orders.Count().ShouldEqual(1);
            context.Orders.First().OrderId.ShouldEqual(orderId);
        }
    }
}