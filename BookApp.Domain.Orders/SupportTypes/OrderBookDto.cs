// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.Domain.Orders.SupportTypes
{
    public readonly struct OrderBookDto
    {
        public BookView Book { get; }
        public short NumBooks { get; }

        public OrderBookDto(BookView book, short numBooks)
        {
            Book = book;
            NumBooks = numBooks;
        }
    }
}