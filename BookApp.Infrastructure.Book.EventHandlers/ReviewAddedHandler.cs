// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.DomainParts;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Book.EventHandlers
{
    public class ReviewAddedHandler : IBeforeSaveEventHandler<BookReviewAddedEvent>
    {
        public IStatusGeneric Handle(EntityEventsBase callingEntity, BookReviewAddedEvent domainEvent)
        {
            //Here is the fast (delta) version of the update. Doesn't need access to the database
            var totalStars = Math.Round(domainEvent.Book.ReviewsAverageVotes * 
                                        domainEvent.Book.ReviewsCount) +
                             domainEvent.NumStars;
            var numReviews = domainEvent.Book.ReviewsCount + 1;
            domainEvent.UpdateReviewCachedValues(numReviews, totalStars / numReviews);

            return null;
        }
    }
}