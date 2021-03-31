// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.UdfsSql.Books.Dtos;

namespace BookApp.ServiceLayer.UdfsSql.Books.QueryObjects
{
    public static class BookUdfsListDtoSelect
    {
        public static IQueryable<UdfsBookListDto> 
            MapBookUdfsToDto(this IQueryable<Book> books) //#A
        {
            return books.Select(p      => new UdfsBookListDto
            {
                BookId         = p.BookId, 
                Title          = p.Title, 
                PublishedOn    = p.PublishedOn, 
                EstimatedDate  = p.EstimatedDate,
                OrgPrice       = p.OrgPrice, 
                ActualPrice    = p.ActualPrice, 
                PromotionText  = p.PromotionalText,
                AuthorsOrdered = UdfDefinitions  //#B
                    .AuthorsStringUdf(p.BookId), //#B
                TagsString     = UdfDefinitions  //#B
                    .TagsStringUdf(p.BookId),    //#B
                ReviewsCount = p.Reviews.Count(),
                ReviewsAverageVotes =
                    p.Reviews.Select(y =>
                        (double?)y.NumStars).Average(),
                ManningBookUrl = p.ManningBookUrl
            });
        }
    }
    /*****************************************************
    #A Updated MapBookToDto method, now called MapBookUdfsToDto
    #B The AuthorsOrdered and TagsString are set to the strings from the UDFs
     * *******************************************************/
}