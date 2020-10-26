// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using BookApp.ServiceLayer.UtfsSql.Books.Dtos;

namespace BookApp.ServiceLayer.UtfsSql.Books.QueryObjects
{

    public static class BookUtfsListDtoSort
    {
        public static IQueryable<UtfsBookListDto> OrderUtfsBooksBy
            (this IQueryable<UtfsBookListDto> books, OrderByOptions orderByOptions)
        {
            switch (orderByOptions)
            {
                case OrderByOptions.SimpleOrder: 
                    return books.OrderByDescending( 
                        x => x.BookId); 
                case OrderByOptions.ByVotes: 
                    return books.OrderByDescending(x => 
                        x.ReviewsAverageVotes); 
                case OrderByOptions.ByPublicationDate: 
                    return books.OrderByDescending( 
                        x => x.PublishedOn); 
                case OrderByOptions.ByPriceLowestFirst: 
                    return books.OrderBy(x => x.ActualPrice); 
                case OrderByOptions.ByPriceHighestFirst: 
                    return books.OrderByDescending( 
                        x => x.ActualPrice); 
                default:
                    throw new ArgumentOutOfRangeException(
                        nameof(orderByOptions), orderByOptions, null);
            }
        }
    }
}