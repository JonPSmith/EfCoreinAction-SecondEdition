// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;


namespace ServiceLayer.BookServices.QueryObjects
{
    public enum BooksFilterBy
    {
        [Display(Name = "All")]
        NoFilter = 0,
        [Display(Name = "By Votes...")]
        ByVotes,
        [Display(Name = "By Year published...")]
        ByPublicationYear
    }

    public static class BookListDtoFilter
    {
        public const string AllBooksNotPublishedString = "Coming Soon";

        public static IQueryable<BookListDto> FilterBooksBy(
            this IQueryable<BookListDto> books, 
            BooksFilterBy filterBy, string filterValue)         //#A
        {
            if (string.IsNullOrEmpty(filterValue))              //#B
                return books;                                   //#B

            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:                    //#C
                    return books;                               //#C
                case BooksFilterBy.ByVotes:
                    var filterVote = int.Parse(filterValue);     //#D
                    return books.Where(x =>                      //#D
                          x.ReviewsAverageVotes > filterVote);   //#D
                case BooksFilterBy.ByPublicationYear:             
                    if (filterValue == AllBooksNotPublishedString)//#E
                        return books.Where(                       //#E
                            x => x.PublishedOn > DateTime.UtcNow);//#E

                    var filterYear = int.Parse(filterValue);      //#F
                    return books.Where(                           //#F
                        x => x.PublishedOn.Year == filterYear     //#F
                          && x.PublishedOn <= DateTime.UtcNow);   //#F
                default:
                    throw new ArgumentOutOfRangeException
                        (nameof(filterBy), filterBy, null);
            }
        }
        /***************************************************************
        #A The method is given both the type of filter and the user selected filter value
        #B If the filter value isn't set then it returns the IQueryable with no change
        #C Same for no filter selected - it returns the IQueryable with no change
        #D The filter by votes is a value and above, e.g. 3 and above. Note: not reviews returns null, and the test is always false
        #E If the "coming soon" was picked then we only return books not yet published
        #F If we have a specific year we filter on that. Note that we also remove future books (in case the user chose this year's date)
         * ************************************************************/
    }
}