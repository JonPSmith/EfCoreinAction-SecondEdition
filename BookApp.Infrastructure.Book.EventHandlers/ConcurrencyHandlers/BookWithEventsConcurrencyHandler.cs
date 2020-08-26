// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License file in the project root for license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace BookApp.Infrastructure.Book.EventHandlers.ConcurrencyHandlers
{
    public static class BookWithEventsConcurrencyHandler
    {
        public static IStatusGeneric HandleCacheValuesConcurrency(this Exception ex, DbContext context)
        {
            var dbUpdateEx = ex as DbUpdateConcurrencyException;
            if (dbUpdateEx == null)
                return null; //can't handle this error
            
            var status = new StatusGenericHandler();
            //There could be multiple books if there was a bulk upload. Unusual, but best to handle it.
            foreach (var entry in dbUpdateEx.Entries)
            {
                if (!(entry.Entity is Domain.Books.Book bookBeingWrittenOut))
                    return null; //This handler only handles Book

                //we read in the book that caused the concurrency issue
                //This MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
                var bookThatCausedConcurrency = context.Set<Domain.Books.Book>().AsNoTracking()
                    .SingleOrDefault(p => p.BookId == bookBeingWrittenOut.BookId);

                if (bookThatCausedConcurrency == null)
                {
                    //The book was deleted so we need to stop the book being written out
                    entry.State = EntityState.Detached;
                    continue;
                }

                var handler = new FixConcurrencyMethods(entry, context);
                handler.CheckFixReviewCacheValues(bookThatCausedConcurrency, bookBeingWrittenOut);
                handler.CheckFixAuthorOrdered(bookThatCausedConcurrency, bookBeingWrittenOut);
            }

            return status; //We return a status with no errors, which tells the caller to retry the SaveChanges
        }
    }
}