// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;

namespace BookApp.Infrastructure.Books.CachedValues.CheckFixCode
{
    public static class CheckFixDtoSelect
    {
        public static IQueryable<CheckFixBookDto> 
            MapBookToDto(this IQueryable<Book> books) 
        {
            return books.Select(p      => new CheckFixBookDto 
            {
                BookId               = p.BookId, 

                AuthorsOrdered       = p.AuthorsOrdered,
                ReviewsCount         = p.ReviewsCount,
                ReviewsAverageVotes  = p.ReviewsAverageVotes,

                RecalcAuthorsOrdered = string.Join(", ", 
                    p.AuthorsLink                          
                        .OrderBy(q=> q.Order)         
                        .Select(q=> q.Author.Name)), 
       
                RecalcReviewsCount   = p.Reviews.Count(), 
                RecalcReviewsAverageVotes =                     
                    p.Reviews.Select(y =>                   
                        (double?)y.NumStars).Average(),

                LastUpdatedUtc       = p.LastUpdatedUtc
            });
        }


    }
}