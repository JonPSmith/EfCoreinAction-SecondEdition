// Copyright (c) 2017 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch04_Order
    {
        [Fact]
        public void TestCreateOrderWithOneLineItems()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            context.SaveChanges();

            //VERIFY
            context.Orders.Count().ShouldEqual(1);
            order.LineItems.First().ChosenBook.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateOrderWithTwoLineItemsDifferentBooks()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    },
                    new LineItem
                    {
                        BookId = 2,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            context.SaveChanges();

            //VERIFY
            context.Orders.Count().ShouldEqual(1);
            order.LineItems.First().ChosenBook.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateOrderWithTwoLineItemsSameBooks()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var order = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    },
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order);
            context.SaveChanges();

            //VERIFY
            context.Orders.Count().ShouldEqual(1);
            order.LineItems.First().ChosenBook.ShouldNotBeNull();
        }

        [Fact]
        public void TestCreateTwoOrderWithOneLineItemSameBooks()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var order1 = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            var order2 = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(order1);
            context.Orders.Add(order2);
            context.SaveChanges();

            //VERIFY
            context.Orders.Count().ShouldEqual(2);
        }

        [Fact]
        public void TestUpdateLineItemInOrder()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            var orderSetup = new Order
            {
                CustomerId = userId,
                LineItems = new List<LineItem>
                {
                    new LineItem
                    {
                        BookId = 1,
                        LineNum = 0,
                        BookPrice = 123,
                        NumBooks = 1
                    }
                }
            };
            context.Orders.Add(orderSetup);
            context.SaveChanges();

            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var order = context.Orders.Include(x => x.LineItems).First();
            order.LineItems = new List<LineItem>
            {
                new LineItem
                {
                    BookId = 1,
                    LineNum = 0,
                    BookPrice = 456,
                    NumBooks = 1
                }
            };
            context.SaveChanges();


            //VERIFY
            context.ChangeTracker.Clear();

            var orderCheck = context.Orders.Include(x => x.LineItems).First();
            orderCheck.LineItems.First().BookPrice.ShouldEqual(456);
        }
    }
}