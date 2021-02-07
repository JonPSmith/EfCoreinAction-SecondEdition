// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BookApp.BizLogic.Orders.BasketServices;
using BookApp.BizLogic.Orders.Orders;
using BookApp.Persistence.EfCoreSql.Orders;
using Microsoft.AspNetCore.Http;

namespace BookApp.ServiceLayer.EfCoreSql.Orders.CheckoutServices.Concrete
{
    //NOTE: This service isn't designed to be created via DI because it takes in the IRequestCookieCollection
    public class CheckoutListService
    {
        private readonly OrderDbContext _context;
        private readonly IRequestCookieCollection _cookiesIn;

        public CheckoutListService(OrderDbContext context, IRequestCookieCollection cookiesIn)
        {
            _context = context;
            _cookiesIn = cookiesIn;
        }

        public ImmutableList<CheckoutItemDto> GetCheckoutList()
        {
            var cookieHandler = new BasketCookie(_cookiesIn);
            var service = new CheckoutCookieService(cookieHandler.GetValue());

            return GetCheckoutList(service.LineItems);
        }

        public ImmutableList<CheckoutItemDto> GetCheckoutList(IImmutableList<OrderLineItem> lineItems)
        {
            var result = new List<CheckoutItemDto>();
            foreach (var lineItem in lineItems)
            {
                result.Add(_context.BookViews.Select(book => new CheckoutItemDto
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    BookPrice = book.ActualPrice,
                    ImageUrl = book.ImageUrl,
                    NumBooks = lineItem.NumBooks
                }).Single(y => y.BookId == lineItem.BookId));
            }
            return result.ToImmutableList();
        }
    }
}