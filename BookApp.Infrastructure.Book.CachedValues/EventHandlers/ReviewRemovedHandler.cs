// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.EventHandlers
{
    public class ReviewRemovedHandler : IBeforeSaveEventHandler<BookReviewRemovedEvent>
    {
        public IStatusGeneric Handle(object callingEntity, BookReviewRemovedEvent domainEvent)
        {
            var book = (Domain.Books.Book)callingEntity;

            var numReviews = book.ReviewsCount - 1;
            var totalStars = Math.Round(book.ReviewsAverageVotes * book.ReviewsCount)
                             - domainEvent.ReviewRemoved.NumStars;
            domainEvent.UpdateReviewCachedValues(numReviews, totalStars / numReviews);

            return null;
        }

    }
}