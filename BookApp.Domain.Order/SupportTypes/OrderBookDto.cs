// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.Domain.Order
{
    public struct OrderBookDto
    {
        public int BookId { get; }

        public decimal SoldPrice { get; }
        public short NumBooks { get; }

        public OrderBookDto(int bookId, decimal soldPrice, short numBooks) : this()
        {
            BookId = bookId;
            SoldPrice = soldPrice;
            NumBooks = numBooks;
        }
    }
}