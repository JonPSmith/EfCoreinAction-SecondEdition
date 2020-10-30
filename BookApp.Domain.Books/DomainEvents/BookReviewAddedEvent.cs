// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books.DomainEvents
{
    public class BookReviewAddedEvent : IEntityEvent
    { 
        public BookReviewAddedEvent(int numStars, Action<int,double> updateReviewCachedValues)
        {
            NumStars = numStars;

            UpdateReviewCachedValues = updateReviewCachedValues;
        }

        public int NumStars { get; }

        public Action<int, double> UpdateReviewCachedValues { get; }
    }
}