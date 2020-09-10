// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.EfCode;
using Test.Chapter12Listings.IntegrationEventEfClasses;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter12Tests
{
    public class TestIntegrationEventDbContext
    {
        [Fact]
        public void TestEventsDbContextSeededOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<IntegrationEventDbContext>();
            using var context = new IntegrationEventDbContext(options, new DummyWarehouseService());

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
            context.Products.Count().ShouldEqual(16);
        }

        [Theory]
        [InlineData(false,1)]
        [InlineData(true, 3)]
        public void TestAddOrderDummyWarehouseCheckOk(bool failInWarehouse, int lineCount)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<IntegrationEventDbContext>();
            using var context = new IntegrationEventDbContext(options, new DummyWarehouseService());
            context.Database.EnsureCreated();

            //ATTEMPT
            context.Add(CreateTestOrder(failInWarehouse));
            context.SaveChanges();

            //VERIFY
            var order = context.Orders.Include(x => x.LineItems).Single();
            order.LineItems.Count.ShouldEqual(lineCount);
        }

        [Fact]
        public void TestAddOrderWithWarehouseCheckOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<IntegrationEventDbContext>();
            using var context = new IntegrationEventDbContext(options, new WarehouseEventHandler());
            context.Database.EnsureCreated();

            //ATTEMPT
            context.Add(CreateTestOrder(false));
            context.SaveChanges();

            //VERIFY
            var order = context.Orders.Include(x => x.LineItems).Single();
            order.LineItems.Count.ShouldEqual(1);
        }

        [Fact]
        public void TestAddOrderWithWarehouseCheckThrowException()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<IntegrationEventDbContext>();
            using var context = new IntegrationEventDbContext(options, new WarehouseEventHandler());
            context.Database.EnsureCreated();

            //ATTEMPT
            context.Add(CreateTestOrder(true));
            var ex = Assert.Throws<OutOfStockException>(() => context.SaveChanges());

            //VERIFY
            ex.Message.ShouldEqual("Order 1: We are out of stock of B2x8Red.Order 1: We only have 20 B1x2Blue in stock");
            context.Orders.Count().ShouldEqual(0);
        }

        private Order CreateTestOrder(bool failInWarehouse)
        {
            var order = new Order
            {
                UserId = Guid.Empty,
                LineItems = new List<LineItem>
                {
                    new LineItem {ProductCode = "B1x6Red", Amount = 1}
                }
            };
            if (failInWarehouse)
            {
                order.LineItems.Add(new LineItem { ProductCode = "B2x8Red", Amount = 1 });
                order.LineItems.Add(new LineItem { ProductCode = "B1x2Blue", Amount = 21 });
            }

            return order;
        }
    }
}