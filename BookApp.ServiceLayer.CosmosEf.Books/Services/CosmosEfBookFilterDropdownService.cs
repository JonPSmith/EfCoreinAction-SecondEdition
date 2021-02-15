// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.AppParts;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.CosmosDirect.Books.Services;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.Azure.Cosmos;

namespace BookApp.ServiceLayer.CosmosEf.Books.Services
{
    public class CosmosEfBookFilterDropdownService : ICosmosEfBookFilterDropdownService
    {
        private readonly CosmosDbContext _db;
        private readonly BookAppSettings _settings;
        private readonly BookDbContext _sqlContext;

        public CosmosEfBookFilterDropdownService(CosmosDbContext db, BookAppSettings settings, BookDbContext sqlContext)
        {
            _db = db;
            _settings = settings;
            _sqlContext = sqlContext;
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
                case BooksFilterBy.ByTags:
                    if (_sqlContext == null)
                        throw new NotImplementedException();
                    return _sqlContext.Tags
                        .Select(x => new DropdownTuple
                        {
                            Value = x.TagId,
                            Text = x.TagId
                        }).ToList();
                case BooksFilterBy.ByPublicationYear:
                    var container = _db.GetCosmosContainerFromDbContext(
                        _settings.CosmosDatabaseName);

                    var comingSoonResultSet = 
                        container.GetItemQueryIterator<int>(
                        new QueryDefinition("SELECT value Count(c) FROM c WHERE" +
                            $" c.YearPublished > {DateTime.Today:yyyy-MM-dd} " +
                            "OFFSET 0 LIMIT 1"));
                    var comingSoon = (await
                            comingSoonResultSet.ReadNextAsync())
                        .First() > 0;

                    var now = DateTime.UtcNow;
                    var resultSet = container.GetItemQueryIterator<int>(
                        new QueryDefinition("SELECT DISTINCT VALUE c.YearPublished " + 
                            $"FROM c WHERE c.YearPublished > {now:yyyy-mm-dd}"));

                    var years = (await resultSet.ReadNextAsync()).ToList();
                    var nextYear = DateTime.UtcNow.AddYears(1).Year;
                    var result = years
                        .Where(x => x < nextYear)
                        .OrderByDescending(x => x)
                        .Select(x => new DropdownTuple
                        {
                            Value = x.ToString(),
                            Text = x.ToString()
                        }).ToList();

                    if (comingSoon)
                        result.Insert(0, new DropdownTuple
                        {
                            Value = DisplayConstants.AllBooksNotPublishedString,
                            Text = DisplayConstants.AllBooksNotPublishedString
                        });

                    //This was the old one that took too long
                    //var now = DateTime.UtcNow;
                    //var comingSoon = await _db.Books
                    //    .FirstOrDefaultAsync(x => x.PublishedOn > now) != null;
                    //var nextYear = DateTime.UtcNow.AddYears(1).Year;
                    //var allYears = await _db.Books
                    //    .Select(x => x.YearPublished)
                    //    .Distinct().ToListAsync();
                    ////see this issue in EF Core about why I had to split the query - https://github.com/aspnet/EntityFrameworkCore/issues/16156
                    //var result = allYears
                    //    .Where(x => x < nextYear)                   
                    //    .OrderByDescending(x => x)                  
                    //    .Select(x => new DropdownTuple              
                    //    {                                           
                    //        Value = x.ToString(),                   
                    //        Text = x.ToString()                     
                    //    }).ToList();                                
                    //if (comingSoon)                                 
                    //    result.Insert(0, new DropdownTuple          
                    //    {
                    //        Value = BookListDtoFilter.AllBooksNotPublishedString,
                    //        Text = BookListDtoFilter.AllBooksNotPublishedString
                    //    });

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