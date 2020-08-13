// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Domain.Orders;
using BookApp.Domain.Orders.SupportTypes;
using BookApp.Persistence.NormalSql.Books;
using BookApp.Persistence.NormalSql.Orders;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceNormalSqlOrders
{
    public class TestOrderDbContext
    {
        [Fact]
        public void TestOrderDbContextEmptyOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
            context.GetTableNamesInSqliteDb().ShouldEqual(new []{"Orders", "sqlite_sequence", "LineItem"});

        }

        [Fact]
        public void TestOrderDbContextWithBooksPartOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));

            //ATTEMPT
            context.Database.EnsureCreated();
            context.BookContextEnsureCreatedOnOrderDb();

            //VERIFY
            context.GetTableNamesInSqliteDb().ShouldEqual(new[] { "Orders", "sqlite_sequence", "LineItem", "Authors", "Books", "BookAuthor", "Review" });
        }


        [Fact]
        public void TestOrderDbContextWithOrderOk()
        {
            //SETUP
            var userId = Guid.NewGuid();
            var options = SqliteInMemory.CreateOptions<OrderDbContext>();
            using var context = new OrderDbContext(options, new FakeUserIdService(userId));
            context.Database.EnsureCreated();
            var bookIds = context.SeedFourBookDdPart().ToList();

            //ATTEMPT
            var status = Order.CreateOrder(userId, new[]
            {
                new OrderBookDto(bookIds[0], 123, 1),
                new OrderBookDto(bookIds[1], 456, 1),
            });
            context.Add(status.Result);
            context.SaveChanges();

            //VERIFY
            context.Orders.Count().ShouldEqual(1);
            context.Set<LineItem>().Count().ShouldEqual(2);
            context.BookViews.Count().ShouldEqual(4);
        }
    }
}