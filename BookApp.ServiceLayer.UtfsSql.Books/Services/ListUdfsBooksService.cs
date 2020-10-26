// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.Common.QueryObjects;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using BookApp.ServiceLayer.UtfsSql.Books.Dtos;
using BookApp.ServiceLayer.UtfsSql.Books.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.UtfsSql.Books.Services
{
    public class ListUdfsBooksService : IListUdfsBooksService
    {
        private readonly BookDbContext _context;

        public ListUdfsBooksService(BookDbContext context)
        {
            _context = context;
        }

        public async Task<IQueryable<UtfsBookListDto>> SortFilterPageAsync
            (SortFilterPageOptions options)
        {
            var preQuery = _context.Books
                .AsNoTracking();
            if (options.FilterBy == BooksFilterBy.ByTags)
            {
                preQuery = preQuery.Where(x =>
                    x.Tags.Select(y => y.TagId)
                        .Contains(options.FilterValue));
            }

             var booksQuery = preQuery
                .MapBookUtfsToDto() 
                .OrderUtfsBooksBy(options.OrderByOptions) 
                .FilterUtfsBooksBy(options.FilterBy, 
                    options.FilterValue); 

            await options.SetupRestOfDtoAsync(booksQuery); 

            return booksQuery.Page(options.PageNum - 1, 
                options.PageSize); 
        }
    }


}