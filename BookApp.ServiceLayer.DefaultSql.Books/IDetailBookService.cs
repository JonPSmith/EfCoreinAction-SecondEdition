// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;

namespace BookApp.ServiceLayer.DefaultSql.Books
{
    public interface IDetailBookService
    {
        Task<BookDetailDto> GetBookDetailAsync(int bookId);
    }
}