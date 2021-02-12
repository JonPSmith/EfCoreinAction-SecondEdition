// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.EventHandlers
{
    public class ReviewAddedHandler :                 
        IBeforeSaveEventHandler<BookReviewAddedEvent> //#A
    {
        public IStatusGeneric Handle(object callingEntity,//#B 
            BookReviewAddedEvent domainEvent)             //#B
        {
            var book = (Domain.Books.Book) callingEntity; //#C

            var totalStars = Math.Round(   //#D
                book.ReviewsAverageVotes   //#D
                    * book.ReviewsCount) + //#D
                domainEvent.NumStars;      //#E
            var numReviews = book.ReviewsCount + 1;  //#F

            domainEvent.UpdateReviewCachedValues( //#G
                numReviews,               //#H
                totalStars / numReviews); //#I

            return null;  //#J
        }
    }
    /***********************************************************
    #A This tells the Event Runner that this event should be called when it finds a BookReviewAddedEvent
    #B The Event Runner provides the instance of the calling entity and the event
    #C This casts the object back to its actual type of Book to make access easier
    #D The first part of this calculation works out how many stars before adding the new stars
    #E Then it adds the star rating from the new Review being added
    #F A simple add of 1 gets the new number of Reviews
    #G The entity class provided a method to update the cached values
    #H The first parameter is the number of reviews
    #I The second parameter provides the new average of the NumStars
    #J Returning null is a quick way to say the event handler is always successful
     ************************************************************/
}