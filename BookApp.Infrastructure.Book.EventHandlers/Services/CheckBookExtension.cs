// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.EventHandlers.Services
{
    public static class CheckBookExtension
    {
        public static IStatusGeneric CheckSingleBook(this Book book, bool fixBadCacheValues)
        {
            var status = new StatusGenericHandler();

            var numReviews = book.Reviews.Count;
            var aveVotes = numReviews == 0 ? 0.0 : book.Reviews.Average(x => x.NumStars);
            if (numReviews != book.ReviewsCount || Math.Abs(aveVotes - book.ReviewsAverageVotes) > 0.0001)
            {
                status.AddError($"BookId: {book.BookId}, Review cached values incorrect\n" +
                                  $"Actual Reviews.Count = {numReviews}, Cached ReviewsCount = {book.ReviewsCount}" +
                                  $"Actual Reviews average = {aveVotes:F5}, Cached ReviewsAverageVotes = {book.ReviewsAverageVotes:F5}" +
                                  $"Last updated {book.LastUpdatedUtc:G}");
                if (fixBadCacheValues)
                {
                    book.UpdateReviewCachedValues(numReviews, aveVotes);
                }
            }

            var authorsString = string.Join(", ", book.AuthorsLink.SelectMany(x => x.Author.Name));
            if (authorsString != book.AuthorsOrdered)
            {
                status.AddError($"BookId: {book.BookId}, AuthorsOrdered cached value incorrect\n" +
                                $"Actual authors string = {authorsString}, Cached AuthorsOrdered = {book.AuthorsOrdered}" +
                                $"Last updated {book.LastUpdatedUtc:G}");

                if (fixBadCacheValues)
                {
                    book.AuthorsOrdered = authorsString;
                }
            }

            return status;
        }
    }
}