// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Persistence.EfCoreSql.Books;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CachedValues.EventHandlers
{
    public class AuthorNameUpdatedHandler : 
        IBeforeSaveEventHandler<AuthorNameUpdatedEvent> //#A
    {
        private readonly BookDbContext _context;  //#B

        public AuthorNameUpdatedHandler//#B
            (BookDbContext context)    //#B
        {                              //#B
            _context = context;        //#B
        }                              //#B

        public IStatusGeneric Handle(object callingEntity,  //#C
            AuthorNameUpdatedEvent domainEvent)             //#C
        {
            var changedAuthor = (Author) callingEntity;     //#D

            foreach (var book in _context.Set<BookAuthor>()     //#E
                .Where(x => x.AuthorId == changedAuthor.AuthorId) //#E
                .Select(x => x.Book))                             //#E
            {
                var allAuthorsInOrder = _context.Books    //#F
                    .Single(x => x.BookId == book.BookId) //#F
                    .AuthorsLink.OrderBy(y => y.Order)    //#F
                    .Select(y => y.Author).ToList();      //#F

                var newAuthorsOrdered =                   //#G
                    string.Join(", ",                     //#G
                        allAuthorsInOrder.Select(x =>     //#G
                    x.AuthorId == changedAuthor.AuthorId  //#H
                        ? changedAuthor.Name              //#H
                        : x.Name));                       //#H

                book.ResetAuthorsOrdered(newAuthorsOrdered);//#I
            }

            return null; //#J
        }
    }
    /***********************************************************
    #A This tells the Event Runner that this event should be called when it finds a AuthorNameUpdatedEvent
    #B The event handler needs to access the database 
    #C The Event Runner provides the instance of the calling entity and the event
    #D This casts the object back to its actual type of Author to make access easier
    #E This loops through all the books that contain the Author that has changed
    #F This gets the Authors, in the correct order, linked to this Book
    #G This will create a comma delimited string with the names from the Authors in the Book
    #H The changed Author's Name hasn't been written to the database yet, so it finds that Author and substitutes the new name 
    #I This updates each Book's AuthorsOrdered property
    #J Returning null is a quick way to say the event handler is always successful
     ************************************************************/
}