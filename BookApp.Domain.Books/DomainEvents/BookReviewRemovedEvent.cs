// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books.DomainEvents
{
    public class BookReviewRemovedEvent : IDomainEvent
    {
        public BookReviewRemovedEvent(Review reviewRemoved, Book book, Action<int, double> updateReviewCachedValues)
        {
            ReviewRemoved = reviewRemoved;
            Book = book;
            UpdateReviewCachedValues = updateReviewCachedValues;
        }

        public Review ReviewRemoved { get; }

        public Book Book { get; }

        public Action<int, double> UpdateReviewCachedValues { get; }
    }
}