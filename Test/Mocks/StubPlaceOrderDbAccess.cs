// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BizDbAccess.Orders;
using DataLayer.EfClasses;
using Test.TestHelpers;

namespace Test.Mocks
{
    public class StubPlaceOrderDbAccess : IPlaceOrderDbAccess
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="createLastInFuture">If true then the last book will be in the future</param>
        /// <param name="promotionPriceForFirstBook">if number it adds a promotion to the first book</param>
        public StubPlaceOrderDbAccess(bool createLastInFuture = false, int? promotionPriceForFirstBook = null)
        {
            var numBooks = createLastInFuture ? DateTime.UtcNow.Year - EfTestData.DummyBookStartDate.Year + 2 : 10;
            var books = EfTestData.CreateDummyBooks(numBooks, createLastInFuture);
            if (promotionPriceForFirstBook != null)
                books.First().Promotion = new PriceOffer
                {
                    NewPrice = (int)promotionPriceForFirstBook,
                    PromotionalText = "Unit Test"
                };
            Books = books.ToImmutableList();
        }

        public ImmutableList<Book> Books { get; private set; }

        public Order AddedOrder { get; private set; }


        /// <summary>
        /// This finds any books that fits the BookIds given to it
        /// and includes any promoptions
        /// </summary>
        /// <param name="bookIds"></param>
        /// <returns>A dictionary with the BookId as the key, and the Book as the value</returns>
        public IDictionary<int, Book> FindBooksByIdsWithPriceOffers(IEnumerable<int> bookIds)
        {
            return Books.AsQueryable().Where(x => bookIds.Contains(x.BookId))
                .ToDictionary(key => key.BookId);
        }

        public void Add(Order newOrder)
        {
            AddedOrder = newOrder;
        }
    }
}