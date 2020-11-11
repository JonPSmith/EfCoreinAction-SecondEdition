// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Domain.Books;
using GenericServices;

namespace BookApp.ServiceLayer.DefaultSql.Books.Dtos
{
    public class SimpleBookList : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public DateTime LastUpdatedUtc { get; set; }

    }
}