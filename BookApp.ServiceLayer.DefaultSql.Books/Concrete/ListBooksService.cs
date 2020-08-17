// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.Common.QueryObjects;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.DefaultSql.Books.Concrete
{
    public class ListBooksService
    {
        private readonly BookDbContext _context;

        public ListBooksService(BookDbContext context)
        {
            _context = context;
        }

        public IQueryable<BookListDto> SortFilterPage
            (SortFilterPageOptions options)
        {
            var booksQuery = _context.Books 
                .AsNoTracking() 
                .MapBookToDto() 
                .OrderBooksBy(options.OrderByOptions) 
                .FilterBooksBy(options.FilterBy, 
                    options.FilterValue); 

            options.SetupRestOfDto(booksQuery); 

            return booksQuery.Page(options.PageNum - 1, 
                options.PageSize); 
        }
    }


}