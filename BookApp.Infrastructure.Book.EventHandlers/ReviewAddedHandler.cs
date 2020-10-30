// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.EventHandlers
{
    public class ReviewAddedHandler : IBeforeSaveEventHandler<BookReviewAddedEvent>
    {
        public IStatusGeneric Handle(object callingEntity, BookReviewAddedEvent domainEvent)
        {
            var book = (Domain.Books.Book) callingEntity;
            //Here is the fast (delta) version of the update. Doesn't need access to the database
            var totalStars = Math.Round(book.ReviewsAverageVotes *
                                        book.ReviewsCount) +
                             domainEvent.NumStars;
            var numReviews = book.ReviewsCount + 1;
            domainEvent.UpdateReviewCachedValues(numReviews, totalStars / numReviews);

            return null;
        }
    }
}