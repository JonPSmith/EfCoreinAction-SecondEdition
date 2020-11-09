// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License file in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookApp.Infrastructure.Books.CachedValues.ConcurrencyHandlers
{
    public class FixConcurrencyMethods
    {
        private readonly EntityEntry _entry;
        private readonly DbContext _context;

        public FixConcurrencyMethods(EntityEntry entry, DbContext context)
        {
            _entry = entry;
            _context = context;
        }

        /// <summary>
        /// This fixes the Review cache values, ReviewsCount and ReviewsAverageVotes, by working out the change that the
        /// two books were trying to apply and combining them into one new update (which will replace what the bookThatCausedConcurrency
        /// wrote to the database.
        /// This uses some maths to do this and has the benefit that it doesn't read the database, which might have changed during the time we do that.
        /// </summary>
        /// <param name="bookThatCausedConcurrency"></param>
        /// <param name="bookBeingWrittenOut"></param>
        public void CheckFixReviewCacheValues( //#A
            Book bookThatCausedConcurrency,  //#B
            Book bookBeingWrittenOut)        //#C
        {
            var previousCount = (int)_entry                  //#D
                .Property(nameof(Book.ReviewsCount))         //#D
                .OriginalValue;                              //#D
            var previousAverageVotes = (double)_entry        //#D
                .Property(nameof(Book.ReviewsAverageVotes))  //#D
                .OriginalValue;                              //#D

            if (previousCount ==                                  //#E
                bookThatCausedConcurrency.ReviewsCount            //#E
                && Math.Abs(previousAverageVotes -                //#E
                    bookThatCausedConcurrency.ReviewsAverageVotes) < 0.0001) //#E
                return;                                           //#E
            
            
            //There was a concurrency issue with the Review cache values
            //In this case we need recompute the Review cache including the bookThatCausedConcurrency changes

            //Get the change that the new book update was trying to apply.
            var previousTotalStars = Math.Round(       //#F
                previousAverageVotes * previousCount); //#F

            //This gets the change that the event was trying at make to the cached values
            var countChange = 
                bookBeingWrittenOut.ReviewsCount         //#G
                - previousCount;           //#G
            var starsChange = Math.Round(                //#G
                  bookBeingWrittenOut.ReviewsAverageVotes  //#G
                  * bookBeingWrittenOut.ReviewsCount)      //#G
              - previousTotalStars;      //#G

            //Now we combine original change in the bookBeingWrittenOut with the bookThatCausedConcurrency changes to get the combined answer.
            var newCount =                                     //#H
                bookThatCausedConcurrency.ReviewsCount         //#H
                + countChange;                                 //#H
            var newTotalStars = Math.Round(                    //#H
                    bookThatCausedConcurrency.ReviewsAverageVotes  //#H
                    * bookThatCausedConcurrency.ReviewsCount)      //#H
                + starsChange;                    //#H

            //We write these combined values into the bookBeingWrittenOut via the entry
            _entry.Property(nameof(Book.ReviewsCount))          //#I
                .CurrentValue = newCount;                       //#I
            _entry.Property(nameof(Book.ReviewsAverageVotes))   //#I
                .CurrentValue = newCount == 0                   //#I
                ? 0 : newTotalStars / newCount;                 //#I

            //Now set the original values to the bookOverwrote so that we won't have another concurrency
            //- unless another update happened while we were fixing this. In which case we get another concurrency to fix in the same way.
            _entry.Property(nameof(Book.ReviewsCount))        //#J
                .OriginalValue = bookThatCausedConcurrency    //#J
                .ReviewsCount;                            //#J
            _entry.Property(nameof(Book.ReviewsAverageVotes)) //#J
                    .OriginalValue =                          //#J
                bookThatCausedConcurrency             //#J
                    .ReviewsAverageVotes;             //#J
        }
        /**********************************************************************
        #A This method handles concurrency errors in the Reviews cached values
        #B This parameter is the Book from the database that caused the concurrency issue
        #C This parameter is the Book you were trying to update
        #D These hold the count and votes in the database before the events changed them
        #E If the previous count and votes match the current database then there is no Review concurrency issue, so it returns
        #F This works out the stars before the new update was applied
        #G This gets the change that the event was trying at make to the cached values
        #H These works out the combined change from the current book and the other updates done to the database
        #I From this you can set the Reviews cached values with the combined values
        #J Finally you need to set the OriginalValues for the Review cached values to the current database
         ******************************************************************/

        /// <summary>
        /// This recomputes the AuthorsOrdered string from the database. But to get the correct answer
        /// we need to use Find, as that will return any entity that is in the DbContext. This picks up the change(s) applied 
        /// </summary>
        /// <param name="bookThatCausedConcurrency"></param>
        /// <param name="bookBeingWrittenOut"></param>
        public void CheckFixAuthorsOrdered( //#A
            Book bookThatCausedConcurrency, //#B
            Book bookBeingWrittenOut)       //#C
        {
            var previousAuthorsOrdered = (string)_entry //#D
                .Property(nameof(Book.AuthorsOrdered))  //#D
                .OriginalValue;                         //#D

            if (previousAuthorsOrdered ==                 //#E
                bookThatCausedConcurrency.AuthorsOrdered) //#E
                return;                                   //#E

            //There was a concurrency issue with the combined string of authors.
            //In this case we need recompute the AuthorsOrdered, but we must use Find so that any outstanding changes will be picked up.

            var allAuthorsIdsInOrder = _context.Set<Book>() //#F
                .IgnoreQueryFilters()                       //#F
                .Where(x => x.BookId ==                     //#F
                            bookBeingWrittenOut.BookId)     //#F
                .Select(x => x.AuthorsLink                  //#F
                    .OrderBy(y => y.Order)                  //#F
                    .Select(y => y.AuthorId)).ToList()      //#F
                .Single();                                  //#F

            //Note the use of Find to get the changed data in the current DbContext
            //That catches any changes that are waiting to be written to the database
            var namesInOrder = allAuthorsIdsInOrder          //#G
                .Select(x => _context.Find<Author>(x).Name); //#G

            var newAuthorsOrdered =                //#H
                string.Join(", ", namesInOrder);   //#H

            //We write the new value into the bookBeingWrittenOut via the entry
            _entry.Property(nameof(Book.AuthorsOrdered)) //#I
                .CurrentValue = newAuthorsOrdered;       //#I

            //Now set the original value to the bookOverwrote so that we won't have another concurrency
            //- unless another update happened while we were fixing this. In which case we get another concurrency to fix in the same way.
            _entry.Property(nameof(Book.AuthorsOrdered))   //#J
                .OriginalValue =                           //#J
                bookThatCausedConcurrency.AuthorsOrdered;  //#J
        }
        /**********************************************************************
        #A This method handles concurrency errors in the AuthorsOrdered cached value
        #B This parameter is the Book from the database that caused the concurrency issue
        #C This parameter is the Book you were trying to update
        #D This gets the previous AuthorsOrdered string before the event updated it
        #E If the previous AuthorsOrdered match the current database AuthorsOrdered then there is no AuthorsOrdered concurrency issue, so it returns
        #F THis gets the AuthorIds for each Author linked to this Book in the correct order 
        #G This gets the Name of each Author using the Find method
        #H Then it is simple to created a comma delimited list of the authors
        #I From this you can set the AuthorsOrdered cached value with the combined values
        #J Finally you need to set the OriginalValues for the AuthorsOrdered cached value to the current database
         ******************************************************************/
    }
}