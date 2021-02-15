// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using Microsoft.Azure.Cosmos;

namespace BookApp.ServiceLayer.CosmosDirect.Books.Services
{
    public static class CosmosDirectFilterDropdown
    {
        public static async Task<IEnumerable<DropdownTuple>> GetFilterDropDownValuesAsync(this CosmosDbContext context,
            BooksFilterBy filterBy, string databaseName)
        {
            var container = context.GetCosmosContainerFromDbContext(databaseName);

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:
                    //return an empty list
                    return new List<DropdownTuple>();
                case BooksFilterBy.ByVotes:
                    return FormVotesDropDown();
                case BooksFilterBy.ByTags:
                    var tagResults = container.GetItemQueryIterator<string>(
                        new QueryDefinition("SELECT DISTINCT value f.TagId FROM c JOIN f in c.Tags"));
                    var tags = (await tagResults.ReadNextAsync()).OrderBy(x => x).ToList();
                    return tags
                        .Select(x => new DropdownTuple
                        {
                            Value = x,
                            Text = x
                        }).ToList();
                case BooksFilterBy.ByPublicationYear:
                    var comingSoonResultSet = container.GetItemQueryIterator<int>(
                        new QueryDefinition($"SELECT value Count(c) FROM c WHERE c.YearPublished > {DateTime.Today:yyyy-MM-dd} OFFSET 0 LIMIT 1"));
                    var comingSoon = (await comingSoonResultSet.ReadNextAsync()).First() > 0;

                    var now = DateTime.UtcNow;
                    var resultSet = container.GetItemQueryIterator<int>(
                        new QueryDefinition($"SELECT DISTINCT VALUE c.YearPublished FROM c WHERE c.YearPublished > {now:yyyy-mm-dd}"));
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