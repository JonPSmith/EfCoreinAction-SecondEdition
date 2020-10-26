// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.UtfsSql.Books.Dtos;

namespace BookApp.ServiceLayer.UtfsSql.Books.QueryObjects
{
    public static class BookUtfsListDtoSelect
    {
        public static IQueryable<UtfsBookListDto> 
            MapBookUtfsToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new UtfsBookListDto
            {
                BookId                 = p.BookId, 
                Title                  = p.Title, 
                PublishedOn            = p.PublishedOn, 
                EstimatedDate          = p.EstimatedDate,
                OrgPrice               = p.OrgPrice, 
                ActualPrice            = p.ActualPrice, 
                PromotionText          = p.PromotionalText,
                AuthorsOrdered         = UdfDefinitions.AuthorsStringUdf(p.BookId),
                TagsString             = UdfDefinitions.TagsStringUdf(p.BookId),
                ReviewsCount = p.Reviews.Count(),
                ReviewsAverageVotes =
                    p.Reviews.Select(y =>
                        (double?)y.NumStars).Average(),
                ManningBookUrl = p.ManningBookUrl
            });
        }
    }
}