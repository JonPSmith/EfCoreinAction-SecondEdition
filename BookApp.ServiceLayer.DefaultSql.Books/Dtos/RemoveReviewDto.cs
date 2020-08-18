// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Domain.Books;
using GenericServices;
using Microsoft.AspNetCore.Mvc;

namespace BookApp.ServiceLayer.DefaultSql.Books.Dtos
{
    [IncludeThen(nameof(Book.Reviews))]
    public class RemoveReviewDto: ILinkToEntity<Book>
    {
        [HiddenInput]
        public int BookId { get; set; }

        public string Title { get; set; }

        public int ReviewId { get; set; }
    }
}