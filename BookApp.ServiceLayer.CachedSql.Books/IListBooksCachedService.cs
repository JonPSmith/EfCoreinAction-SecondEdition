// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;

namespace BookApp.ServiceLayer.CachedSql.Books
{
    public interface IListBooksCachedService
    {
        Task<IQueryable<BookListDto>> SortFilterPageAsync
            (SortFilterPageOptions options);
    }
}