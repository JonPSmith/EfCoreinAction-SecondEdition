// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DisplayCommon.Books;

namespace BookApp.ServiceLayer.CosmosEf.Books.QueryObjects
{
    public static class CosmosEfBookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<CosmosBook> FilterBooksBy(
            this IQueryable<CosmosBook> books,
            BooksFilterBy filterBy, string filterValue)         
        {
            if (string.IsNullOrEmpty(filterValue))              
                return books;                                   

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:                    
                    return books;                               
                case BooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     
                    return books.Where(x => x.ReviewsAverageVotes > filterVote);
                case BooksFilterBy.ByTags:
                    return books.Where(x => x.TagsString.Contains($"| {filterValue} |"));
                case BooksFilterBy.ByPublicationYear:
                    var now = DateTime.UtcNow;
                    if (filterValue == AllBooksNotPublishedString)
                        return books.Where(
                            x => x.PublishedOn > now);

                    var filterYear = int.Parse(filterValue);
                    return books.Where(
                        x => x.YearPublished == filterYear
                             && x.PublishedOn <= now);
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
    }
}