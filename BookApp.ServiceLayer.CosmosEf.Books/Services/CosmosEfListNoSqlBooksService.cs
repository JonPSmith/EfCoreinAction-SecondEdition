// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.Common.QueryObjects;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.ServiceLayer.CosmosEf.Books.QueryObjects;
using BookApp.ServiceLayer.DisplayCommon.Books;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.CosmosEf.Books.Services
{
    public class CosmosEfListNoSqlBooksService : ICosmosEfListNoSqlBooksService
    {
        private readonly CosmosDbContext _context;

        public CosmosEfListNoSqlBooksService(CosmosDbContext context)
        {
            _context = context;
        }

        public async Task<IList<CosmosBook>> SortFilterPageAsync(SortFilterPageOptionsNoCount options)
        {
            var booksFound = await _context.Books
                .AsNoTracking()                                             
                .OrderBooksBy(options.OrderByOptions)  
                .FilterBooksBy(options.FilterBy,       
                               options.FilterValue)
                .Page(options.PageNum - 1,options.PageSize)
                .ToListAsync();   

            options.SetupRestOfDto(booksFound.Count);

            var x = _context.ChangeTracker;

            return booksFound;
        }
    }

}