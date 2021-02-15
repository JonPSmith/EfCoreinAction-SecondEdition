// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;

namespace BookApp.ServiceLayer.CachedSql.Books.QueryObjects
{
    public static class BookListCachedDtoSelect
    {
        public static IQueryable<BookListDto> 
            MapBookCachedToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new BookListDto
            {
                BookId                 = p.BookId, 
                Title                  = p.Title, 
                PublishedOn            = p.PublishedOn, 
                EstimatedDate          = p.EstimatedDate,
                OrgPrice               = p.OrgPrice, 
                ActualPrice            = p.ActualPrice, 
                PromotionText          = p.PromotionalText,
                ManningBookUrl         = p.ManningBookUrl,
                
                TagStrings             = p.Tags.Select(x => x.TagId).ToArray(),
                
                AuthorsOrdered         = p.AuthorsOrdered,
                ReviewsCount           = p.ReviewsCount,
                ReviewsAverageVotes    = p.ReviewsAverageVotes
            });
        }

        /*********************************************************
        #A This method takes in IQueryable<Book> and returns IQueryable<BookListDto>
        #B These are simple copies of existing columns in the Books table
        #C This calculates the selling price, which is the normal price, or the promotion price if that relationship exists 
        #D The PromotionalText depends on whether a PriceOffer exists for this book
        #E This obtains an array of Authors' names, in the right order. We are using a Client vs. Server evaluation as we want the author's names combined into one string
        #F We need to calculate how many reviews there are
        #G To get EF Core to turn the LINQ average into the SQL AVG command I need to cast the NumStars to (double?)
        * *******************************************************/
    }
}