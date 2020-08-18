// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.Orders.BizLogic.Orders;
using BookApp.Infrastructure.Orders.BizLogic.Orders.Concrete;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess.Orders;
using Microsoft.EntityFrameworkCore;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureOrdersBizLogic
{
    public class TestPlaceOrderAction
    {
        [Fact]
        public async Task TestCreateOrderOneBookOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));

            context.Database.EnsureCreated();
            var bookIds = context.SeedFourBookDdPartWithOptionalDbSchemaAdd(true);
            var service = new PlaceOrderAction(new PlaceOrderDbAccess(context));

            //ATTEMPT
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 4},
            };
            var dto = new PlaceOrderInDto(true, userId, lineItems.ToImmutableList());
            var status = await service.CreateOrderAndSaveAsync(dto);

            //VERIFY
            status.IsValid.ShouldBeTrue(status.GetAllErrors());
            context.ChangeTracker.Clear();
            var order = context.Orders.Include(x => x.LineItems).Single();
            var lineItem = order.LineItems.Single();
            lineItem.BookId.ShouldEqual(1);
            lineItem.NumBooks.ShouldEqual((short)4);
            lineItem.BookPrice.ShouldEqual(40);
        }

        [Fact]
        public async Task TestCreateOrderTAndCsBad()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));

            context.Database.EnsureCreated();
            var service = new PlaceOrderAction(new PlaceOrderDbAccess(context));

            //ATTEMPT
            var lineItems = new List<OrderLineItem>
            {
                new OrderLineItem {BookId = 1, NumBooks = 4},
            };
            var dto = new PlaceOrderInDto(false, userId, lineItems.ToImmutableList());
            var status = await service.CreateOrderAndSaveAsync(dto);

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.GetAllErrors().ShouldEqual("You must accept the T&Cs to place an order.");
        }

        [Fact]
        public async Task TestCreateOrderEmptyBasketBad()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));

            context.Database.EnsureCreated();
            var service = new PlaceOrderAction(new PlaceOrderDbAccess(context));

            //ATTEMPT
            var dto = new PlaceOrderInDto(true, userId, (new List<OrderLineItem>()).ToImmutableList());
            var status = await service.CreateOrderAndSaveAsync(dto);

            //VERIFY
            status.IsValid.ShouldBeFalse();
            status.GetAllErrors().ShouldEqual("No items in your basket.");
        }
    }
}