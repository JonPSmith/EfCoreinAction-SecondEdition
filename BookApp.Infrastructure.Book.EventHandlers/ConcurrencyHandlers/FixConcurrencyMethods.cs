// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License file in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BookApp.Infrastructure.Book.EventHandlers.ConcurrencyHandlers
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
        public void CheckFixReviewCacheValues(Domain.Books.Book bookThatCausedConcurrency, Domain.Books.Book bookBeingWrittenOut)
        {
            var previousCount = (int)_entry.Property(nameof(Domain.Books.Book.ReviewsCount)).OriginalValue;
            var previousAverageVotes = (double)_entry.Property(nameof(Domain.Books.Book.ReviewsAverageVotes)).OriginalValue;

            if (previousCount != bookThatCausedConcurrency.ReviewsCount ||
                previousAverageVotes != bookThatCausedConcurrency.ReviewsAverageVotes)
            {
                //There was a concurrency issue with the Review cache values
                //In this case we need recompute the Review cache including the bookThatCausedConcurrency changes

                //Get the change that the new book update was trying to apply.
                var previousTotalStars = Math.Round(previousAverageVotes * previousCount);
                var countChange = bookBeingWrittenOut.ReviewsCount - previousCount;
                var starsChange = Math.Round(bookBeingWrittenOut.ReviewsAverageVotes * bookBeingWrittenOut.ReviewsCount) - previousTotalStars;

                //Now we combine original change in the bookBeingWrittenOut with the bookThatCausedConcurrency changes to get the combined answer.
                var newCount = bookThatCausedConcurrency.ReviewsCount + countChange;
                var totalStars = Math.Round(bookThatCausedConcurrency.ReviewsAverageVotes * bookThatCausedConcurrency.ReviewsCount) +
                                 starsChange;

                //We write these combined values into the bookBeingWrittenOut via the entry
                _entry.Property(nameof(Domain.Books.Book.ReviewsCount)).CurrentValue = newCount;
                _entry.Property(nameof(Domain.Books.Book.ReviewsAverageVotes)).CurrentValue = totalStars / newCount;

                //Now set the original values to the bookOverwrote so that we won't have another concurrency
                //- unless another update happened while we were fixing this. In which case we get another concurrency to fix in the same way.
                _entry.Property(nameof(Domain.Books.Book.ReviewsCount)).OriginalValue = bookThatCausedConcurrency.ReviewsCount;
                _entry.Property(nameof(Domain.Books.Book.ReviewsAverageVotes)).OriginalValue =
                    bookThatCausedConcurrency.ReviewsAverageVotes;
            }
        }

        /// <summary>
        /// This recomputes the AuthorsOrdered string from the database. But to get the correct answer
        /// we need to use Find, as that will return any entity that is in the DbContext. This picks up the change(s) applied 
        /// </summary>
        /// <param name="bookThatCausedConcurrency"></param>
        /// <param name="bookBeingWrittenOut"></param>
        public void CheckFixAuthorOrdered(Domain.Books.Book bookThatCausedConcurrency, Domain.Books.Book bookBeingWrittenOut)
        {
            var previousAuthorsOrdered = (string)_entry.Property(nameof(Domain.Books.Book.AuthorsOrdered)).OriginalValue;

            if (previousAuthorsOrdered != bookThatCausedConcurrency.AuthorsOrdered)
            {
                //There was a concurrency issue with the combined string of authors.
                //In this case we need recompute the AuthorsOrdered, but we must use Find so that any outstanding changes will be picked up.

                var allAuthorsIdsInOrder = _context.Set<Domain.Books.Book>()
                    .Where(x => x.BookId == bookBeingWrittenOut.BookId)
                    .Select(x => x.AuthorsLink.OrderBy(y => y.Order).Select(y => y.AuthorId)).ToList()
                    .Single();

                //Note the use of Find to get the changed data in the current DbContext
                //That catches any changes that are waiting to be written to the database
                var namesInOrder = allAuthorsIdsInOrder.Select(x => _context.Find<Author>(x).Name);
                var newAuthorsOrdered = string.Join(", ", namesInOrder);

                //We write the new value into the bookBeingWrittenOut via the entry
                _entry.Property(nameof(Domain.Books.Book.AuthorsOrdered)).CurrentValue = newAuthorsOrdered;

                //Now set the original value to the bookOverwrote so that we won't have another concurrency
                //- unless another update happened while we were fixing this. In which case we get another concurrency to fix in the same way.
                _entry.Property(nameof(Domain.Books.Book.AuthorsOrdered)).OriginalValue = bookThatCausedConcurrency.AuthorsOrdered;
            }
        }
    }
}