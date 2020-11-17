// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;

namespace BookApp.ServiceLayer.CosmosEf.Books.QueryObjects
{
    public static class BookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<CosmosBook> FilterBooksBy(
            this IQueryable<CosmosBook> books, 
            CosmosBooksFilterBy filterBy, string filterValue)         
        {
            if (string.IsNullOrEmpty(filterValue))              
                return books;                                   

            switch (filterBy)
            {
                case CosmosBooksFilterBy.NoFilter:                    
                    return books;                               
                case CosmosBooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     
                    return books.Where(x => x.ReviewsAverageVotes > filterVote);
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
    }
}