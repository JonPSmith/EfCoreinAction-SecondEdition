// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Domain.Books;
using GenericServices;

namespace BookApp.ServiceLayer.DefaultSql.Books.Dtos
{
    public class AlterSoftDeleteDto : ILinkToEntity<Book>
    {
        public int BookId { get; set; }
        public bool SoftDeleted { get; set; }
    }
}