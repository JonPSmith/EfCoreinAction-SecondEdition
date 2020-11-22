// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericEventRunner.ForHandlers;
using StatusGeneric;
using System;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;

namespace BookApp.Infrastructure.Books.CosmosDb.EventsHandlers
{
    public class BookChangeHandlerAsync 
        : IDuringSaveEventHandlerAsync<BookChangedEvent> //#A
    {
        private readonly IBookToCosmosBookService _service; //#B

        public BookChangeHandlerAsync(
            IBookToCosmosBookService service)
        {
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(       //#C
            object callingEntity, BookChangedEvent domainEvent, 
            Guid uniqueKey)
        {
            var bookId = ((Book)callingEntity).BookId; //#D
            switch (domainEvent.BookChangeType) //#E
            {
                case BookChangeTypes.Added:
                    await _service.AddCosmosBookAsync(bookId); //#F
                    break;
                case BookChangeTypes.Updated:
                    await _service.UpdateCosmosBookAsync(bookId); //#G
                    break;
                case BookChangeTypes.Deleted:
                    await _service.DeleteCosmosBookAsync(bookId); //#H
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null; //#I
        }
    }
    /***********************************************************
    #A This defines the class as a During (integration) event for the BookChanged event
    #B This service provides the code to Add, Update, and Delete a CosmosBook
    #C The event handler uses async, as Cosmos DB uses async
    #D This extracts the BookId from the calling entity, which is a Book.
    #E The BookChangeType can be Added, updated or Deleted
    #F This calls the Add part of the service, with the BookId of the SQL Book
    #G This calls the Update part of the service, with the BookId of the SQL Book
    #H This calls the Delete part of the service, with the BookId of the SQL Book
    #I Retuning null tells the GenericEventRunner that this method is always successful
     ************************************************************/
}