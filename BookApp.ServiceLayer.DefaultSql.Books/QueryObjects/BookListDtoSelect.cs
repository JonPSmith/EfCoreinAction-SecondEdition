// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;

namespace BookApp.ServiceLayer.DefaultSql.Books.QueryObjects
{
    public static class BookListDtoSelect
    {
        public static IQueryable<BookListDto> 
            MapBookToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new BookListDto //#A
            {
                BookId                 = p.BookId,           //#A
                Title                  = p.Title,            //#A
                PublishedOn            = p.PublishedOn,      //#A
                EstimatedDate          = p.EstimatedDate,    //#A
                OrgPrice               = p.OrgPrice,         //#B
                ActualPrice            = p.ActualPrice,      //#B
                PromotionText          = p.PromotionalText,  //#B
                AuthorsOrdered         = string.Join(", ", //#C
                    p.AuthorsLink                          //#C
                        .OrderBy(q     => q.Order)         //#C
                        .Select(q      => q.Author.Name)), //#C
                TagStrings             = p.Tags            //#C
                    .Select(x => x.TagId).ToArray(),       //#C
                ReviewsCount           = p.Reviews.Count(), //#D
                ReviewsAverageVotes    =                    //#D
                    p.Reviews.Select(y =>                   //#D
                        (double?)y.NumStars).Average(),     //#D
                ManningBookUrl         = p.ManningBookUrl
            });
        }

        /*********************************************************
        #A Good practice: Only load the properties you need
        #B Good practice: Part 3 uses DDD so that the price promotion alters the ActualPrice 
        #C Good practice: Don't load the whole relationships, but just the parts you need 
        #D Good practice: The ReviewsCount and ReviewsAverageVotes are calculated in the database
        * *******************************************************/
    }
}