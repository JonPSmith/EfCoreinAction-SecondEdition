// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using ServiceLayer.BookServices.QueryObjects;

namespace ServiceLayer.BookServices.Concrete
{
    public class BookFilterDropdownService
    {
        private readonly EfCoreContext _db;

        public BookFilterDropdownService(EfCoreContext db)
        {
            _db = db;
        }

        /// <summary>
        ///     This makes the various Value + text to go in the dropdown based on the FilterBy option
        /// </summary>
        /// <param name="filterBy"></param>
        /// <returns></returns>
        public IEnumerable<DropdownTuple> GetFilterDropDownValues(BooksFilterBy filterBy)
        {
            switch (filterBy)
            {
                case BooksFilterBy.NoFilter:
                    //return an empty list
                    return new List<DropdownTuple>();
                case BooksFilterBy.ByVotes:
                    return FormVotesDropDown();
                case BooksFilterBy.ByTags:
                    return _db.Tags
                        .Select(x => new DropdownTuple
                        {
                            Value = x.TagId,
                            Text = x.TagId
                        }).ToList();
                case BooksFilterBy.ByPublicationYear:
                    var result = _db.Books                           //#A
                        .Where(x => x.PublishedOn <= DateTime.Today) //#A
                        .Select(x => x.PublishedOn.Year)             //#A
                        .Distinct()                                  //#A
                        .OrderByDescending(x => x)                   //#B
                        .Select(x => new DropdownTuple               //#C
                        {                                            //#C
                            Value = x.ToString(),                    //#C
                            Text = x.ToString()                      //#C
                        }).ToList();                                 //#C
                    var comingSoon = _db.Books.                      //#D
                        Any(x => x.PublishedOn > DateTime.Today);   //#D
                    if (comingSoon)                                  //#E
                        result.Insert(0, new DropdownTuple           //#E
                        {
                            Value = BookListDtoFilter.AllBooksNotPublishedString,
                            Text = BookListDtoFilter.AllBooksNotPublishedString
                        });

                    return result;
                /*****************************************************************
                #A This long command gets the year of publication by filters out the future books, select the data and uses distinct to only have one of each year
                #B Orders the years, with newest year at the top
                #C I finally use two client/server evaluations to turn the values into strings
                #D This returns true if there is a book in the list that is not yet published
                #E Finally I add a "coming soon" filter for all the future books
                 * ***************************************************************/
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