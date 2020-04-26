// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizLogic.BasketServices;
using BizLogic.Orders;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Http;

namespace ServiceLayer.CheckoutServices.Concrete
{
    public class CheckoutListService
    {
        private readonly EfCoreContext _context;
        private readonly IRequestCookieCollection _cookiesIn;

        public CheckoutListService(EfCoreContext context, IRequestCookieCollection cookiesIn)
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
                result.Add(_context.Books.Select(book => new CheckoutItemDto
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    AuthorsName = string.Join(", ",
                        book.AuthorsLink
                            .OrderBy(q => q.Order)
                            .Select(q => q.Author.Name)),
                    BookPrice = book.Promotion == null ? book.Price : book.Promotion.NewPrice,
                    ImageUrl = book.ImageUrl,
                    NumBooks = lineItem.NumBooks
                }).Single(y => y.BookId == lineItem.BookId));
            }
            return result.ToImmutableList();
        }
    }
}