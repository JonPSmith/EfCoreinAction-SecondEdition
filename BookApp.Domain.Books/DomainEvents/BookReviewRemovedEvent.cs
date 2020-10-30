// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books.DomainEvents
{
    public class BookReviewRemovedEvent : IEntityEvent
    {
        public BookReviewRemovedEvent(Review reviewRemoved, Action<int, double> updateReviewCachedValues)
        {
            ReviewRemoved = reviewRemoved;
            UpdateReviewCachedValues = updateReviewCachedValues;
        }

        public Review ReviewRemoved { get; }

        public Action<int, double> UpdateReviewCachedValues { get; }
    }
}