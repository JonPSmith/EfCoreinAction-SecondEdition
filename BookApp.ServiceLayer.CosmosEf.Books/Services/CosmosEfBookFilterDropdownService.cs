// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace BookApp.ServiceLayer.CosmosEf.Books.Services
{
    public class CosmosEfBookFilterDropdownService : ICosmosEfBookFilterDropdownService
    {
        private readonly CosmosDbContext _db;

        public CosmosEfBookFilterDropdownService(CosmosDbContext db)
        {
            _db = db;
        }

        /// <summary>
        /// This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DropdownTuple>> GetFilterDropDownValuesAsync(BooksFilterBy filterBy)
        {
            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:
                    //return an empty list
                    return new List<DropdownTuple>();
                case BooksFilterBy.ByVotes:
                    return FormVotesDropDown();
                case BooksFilterBy.ByPublicationYear:
                    var now = DateTime.UtcNow;
                    var comingSoon = _db.Books.Where(x => x.PublishedOn > now).Select(_ => 1).AsEnumerable().Any();  
                    var nextYear = DateTime.UtcNow.AddYears(1).Year;
                    var allYears = await _db.Books
                        .Select(x => x.YearPublished)
                        .Distinct().ToListAsync();
                    //see this issue in EF Core about why I had to split the query - https://github.com/aspnet/EntityFrameworkCore/issues/16156
                    var result = allYears.Where(x => x < nextYear)                   
                        .OrderByDescending(x => x)                  
                        .Select(x => new DropdownTuple              
                        {                                           
                            Value = x.ToString(),                   
                            Text = x.ToString()                     
                        }).ToList();                                
                    if (comingSoon)                                 
                        result.Insert(0, new DropdownTuple          
                        {
                            Value = BookListDtoFilter.AllBooksNotPublishedString,
                            Text = BookListDtoFilter.AllBooksNotPublishedString
                        });

                    return result;
                default:
                    throw new ArgumentOutOfRangeException(nameof(filterBy), filterBy, null);
            }
        }

        //------------------------------------------------------------
        // private methods

        private static IEnumerable<DropdownTuple> FormVotesDropDown()
        {
            return new[]
            {
                new DropdownTuple {Value = "4", Text = "4 stars and up"},
                new DropdownTuple {Value = "3", Text = "3 stars and up"},
                new DropdownTuple {Value = "2", Text = "2 stars and up"},
                new DropdownTuple {Value = "1", Text = "1 star and up"},
            };
        }
    }
}