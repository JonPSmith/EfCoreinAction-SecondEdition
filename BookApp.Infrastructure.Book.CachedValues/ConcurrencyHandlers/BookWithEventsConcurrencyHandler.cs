// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License file in the project root for license information.

using System;
using System.Linq;
using BookApp.Domain.Books;
using Microsoft.EntityFrameworkCore;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.ConcurrencyHandlers
{
    public static class BookWithEventsConcurrencyHandler
    {
        public static IStatusGeneric HandleCacheValuesConcurrency //#A
            (this Exception ex, DbContext context)
        {
            var dbUpdateEx = ex as DbUpdateConcurrencyException; //#B
            if (dbUpdateEx == null) //#C
                return null; //#C
            
            //There could be multiple books if there was a bulk upload. Unusual, but best to handle it.
            foreach (var entry in dbUpdateEx.Entries) //#D
            {
                if (!(entry.Entity is Book bookBeingWrittenOut)) //#E
                    return null; //#E

                //we read in the book that caused the concurrency issue
                //This MUST be read as NoTracking otherwise it will interfere with the same entity we are trying to write
                var bookThatCausedConcurrency = context.Set<Book>() //#F
                    .IgnoreQueryFilters()                           //#F
                    .AsNoTracking()                                 //#F
                    .SingleOrDefault(p => p.BookId                  //#F
                        == bookBeingWrittenOut.BookId);             //#F

                if (bookThatCausedConcurrency == null)    //#G
                {                                         //#G
                    entry.State = EntityState.Detached;   //#G
                    continue;                             //#G
                }                                         //#G

                var handler = new FixConcurrencyMethods(entry, context); //#H

                handler.CheckFixReviewCacheValues(                  //#I
                    bookThatCausedConcurrency, bookBeingWrittenOut);//#I

                handler.CheckFixAuthorsOrdered(                      //#J
                    bookThatCausedConcurrency, bookBeingWrittenOut);//#J
            }

            return new StatusGenericHandler(); //#K
        }
        /******************************************************************
        #A This extension method handles the Reviews and Author cached values concurrency issues
        #B We cast the exception to a DbUpdateConcurrencyException
        #C If the exception isn't a DbUpdateConcurrencyException we return null to say we can't handle that exception
        #D It should only one entity, but we handle many entities in case of bulk loading
        #E We case the entity to a Book. If its isn't a Book we return null to say the method can't handle that
        #F You read in a non-tracked version of the Book from the database (note the IgnoreQueryFilters)
        #G If there was no book it was deleted, so we mark the current books as detached, so it won't be updated
        #H This creates the class containing the Reviews and AuthorsOrdered cached values
        #I This fixes any concurrency issues with the Reviews cached values
        #J This fixes any concurrency issues with the AuthorsOrdered cached value
        #K It returns a valid status to say it successfully fixed the concurrency issue
         *****************************************************************/
    }
}