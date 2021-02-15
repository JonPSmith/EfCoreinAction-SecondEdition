// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.Common.QueryObjects;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.CachedSql.Books.QueryObjects;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.CachedSql.Books.Services
{
    public class ListBooksCachedNoCountService : IListBooksCachedNoCountService
    {
        private readonly BookDbContext _context;

        public ListBooksCachedNoCountService(BookDbContext context)
        {
            _context = context;
        }

        public IQueryable<BookListDto> SortFilterPage
            (SortFilterPageOptionsNoCount options)
        {
            var booksQuery = _context.Books
                .AsNoTracking()
                .MapBookCachedToDto()
                .OrderBooksBy(options.OrderByOptions)
                .FilterBooksBy(options.FilterBy, options.FilterValue)
                .Page(options.PageNum - 1, options.PageSize);

            return booksQuery;
        }
    }


}