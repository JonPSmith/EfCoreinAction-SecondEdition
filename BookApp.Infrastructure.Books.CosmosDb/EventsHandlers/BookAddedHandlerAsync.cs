// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericEventRunner.ForHandlers;
using StatusGeneric;
using System;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using NetCore.AutoRegisterDi;

namespace BookApp.Infrastructure.Books.CosmosDb.EventsHandlers
{
    [DoNotAutoRegister]
    public class BookAddedHandlerAsync 
        : IDuringSaveEventHandlerAsync<BookAddedEvent> //#A
    {
        private readonly IBookToCosmosBookService _service; //#B

        public BookAddedHandlerAsync(
            IBookToCosmosBookService service)
        {
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(       //#C
            object callingEntity, BookAddedEvent domainEvent, 
            Guid uniqueKey)
        {
            var bookId = ((Book)callingEntity).BookId; //#D
            await _service.AddCosmosBookAsync(bookId); //#D

            return null;
        }
    }
    /***********************************************************
    #A This defines the class as a During (integration) event for the BookAdded event
    #B This service provides the code to Add, Update, and Delete a CosmosBook
    #C The event handler uses async, as Cosmos DB uses async
    #C This calls the Add part of the service, with the BookId of the SQL Book
    #D Retuning null tells the GenericEventRunner that this method is always successful
     ************************************************************/
}