// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
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
    public class Ch07_TestAutoQueryFilterConfig
    {
        [Fact]
        public void TestSoftDeleteQueryFilter()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                context.Books.First().SoftDeleted = true;
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestUserIdQueryFilter()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options, new FakeUserIdService(userId)))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var order1 = new Order
                {
                    UserId = userId,
                    LineItems = new List<LineItem>
                    {
                        new LineItem {BookId = 1, LineNum = 0, BookPrice = 123, NumBooks = 1}
                    }
                };
                var order2 = new Order
                {
                    UserId = Guid.NewGuid(),
                    LineItems = new List<LineItem>
                    {
                        new LineItem {BookId = 1, LineNum = 0, BookPrice = 123, NumBooks = 1}
                    }
                };
                context.Orders.AddRange(order1, order2);
                context.SaveChanges();

                //ATTEMPT
                var orders = context.Orders.ToList();

                //VERIFY
                orders.Count.ShouldEqual(1);
                orders.Single().UserId.ShouldEqual(userId);
            }
        }

        [Fact]
        public void TestUserIdQueryFilterHandlesUserPerDbContextInstance()
        {
            //SETUP
            var userId1 = Guid.NewGuid();
            var userId2 = Guid.NewGuid();
            DbConnection connection;
            var options1 = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options1, new FakeUserIdService(userId1)))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
                connection = context.Database.GetDbConnection();

                var order1 = new Order
                {
                    UserId = userId1,
                    LineItems = new List<LineItem>
                    {
                        new LineItem {BookId = 1, LineNum = 0, BookPrice = 123, NumBooks = 1}
                    }
                };
                var order2 = new Order
                {
                    UserId = userId2,
                    LineItems = new List<LineItem>
                    {
                        new LineItem {BookId = 1, LineNum = 0, BookPrice = 123, NumBooks = 1}
                    }
                };
                context.Orders.AddRange(order1, order2);
                context.SaveChanges();

                context.Orders.Single().UserId.ShouldEqual(userId1);
            }
            //ATTEMPT
            var options2 = SqliteInMemory.CreateOptions<EfCoreContext>(builder => builder.UseSqlite(connection));
            using (var context = new EfCoreContext(options2, new FakeUserIdService(userId2)))
            {
                var orders = context.Orders.ToList();

                //VERIFY
                orders.Count.ShouldEqual(1);
                orders.Single().UserId.ShouldEqual(userId2);
            }
        }
    }
}