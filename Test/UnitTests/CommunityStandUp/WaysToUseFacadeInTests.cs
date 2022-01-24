// Copyright (c) 2022 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using BookApp.BizLogic.Orders.Orders;
using BookApp.BizLogic.Orders.Orders.Concrete;
using BookApp.Domain.Books;
using BookApp.Domain.Orders;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess.Orders;
using Microsoft.EntityFrameworkCore;
using Test.Mocks;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.CommunityStandUp;

public class WaysToUseFacadeInTests
{
    public class StubPlaceOrderDbAccess : IPlaceOrderDbAccess
    {
        public StubPlaceOrderDbAccess(bool createLastInFuture = false, int? promotionPriceForFirstBook = null)
        {
            var numBooks = createLastInFuture ? DateTime.UtcNow.Year - BookTestData.DummyBookStartDate.Year + 2 : 10;
            var books = BookTestData.CreateDummyBooks(numBooks, createLastInFuture);
            if (promotionPriceForFirstBook != null)
                books.First().AddPromotion(22, "Unit Test");
            Books = books.ToImmutableList();
        }

        public ImmutableList<Book> Books { get; private set; }

        public Order AddedOrder { get; private set; }

        public Task<IDictionary<int, BookView>> FindBooksByIdsAsync(IEnumerable<int> bookIds)
        {
            var bookId = 1;
            var directory = new Dictionary<int, BookView>();
            foreach (var book in Books)
            {
                directory[bookId] = new BookView(bookId, book.Title, book.AuthorsOrdered, book.ActualPrice, book.ImageUrl);
                bookId++;
            }
            return Task.FromResult<IDictionary<int, BookView>>(directory);
        }

        public Task AddAndSave(Order newOrder)
        {
            AddedOrder = newOrder;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task TestCreateOrderOneBookOk()
    {
        //SETUP
        var userId = Guid.NewGuid();
        var options = SqliteInMemory.CreateOptions<OrderDbContext>();
        using var context = new OrderDbContext(options, new FakeUserIdService(userId));
        context.Database.EnsureCreated();

        var stubDbA = new StubPlaceOrderDbAccess();
        var service = new PlaceOrderBizLogic(stubDbA);
        //ATTEMPT
        var lineItems = new List<OrderLineItem>
        {
            new OrderLineItem {BookId = 1, NumBooks = 4},
        };
        var dto = new PlaceOrderInDto(true, userId, lineItems.ToImmutableList());
        var status = await service.CreateOrderAndSaveAsync(dto);

        //VERIFY
        status.IsValid.ShouldBeTrue(status.GetAllErrors());
        stubDbA.AddedOrder.UserId.ShouldEqual(userId);
    }
}