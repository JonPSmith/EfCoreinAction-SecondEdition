// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;
using BookApp.Domain.Orders.SupportTypes;

namespace BookApp.Domain.Orders
{
    public class LineItem 
    {
        internal LineItem(OrderBookDto bookOrder, byte lineNum)
        {
            NumBooks = bookOrder.NumBooks;
            BookId = bookOrder.Book.BookId;
            BookPrice = bookOrder.Book.ActualPrice;
            BookView = bookOrder.Book;
            LineNum = lineNum;
        }

        /// <summary>
        /// Used by EF Core
        /// </summary>
        private LineItem() { }

        public int LineItemId { get; private set; }

        [Range(1,5, ErrorMessage = "This order is over the limit of 5 books.")] 
        public byte LineNum { get; private set; }

        public short NumBooks { get; private set; }

        /// <summary>
        /// This holds a copy of the book price. We do this in case the price of the book changes,
        /// e.g. if the price was discounted in the future the order is still correct.
        /// </summary>
        public decimal BookPrice { get; private set; }

        // relationships

        public int OrderId { get; private set; }
        public int BookId { get; private set; }

        public BookView BookView { get; private set; }
    }

}