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
    public class BookAddedHandlerAsync : IDuringSaveEventHandlerAsync<BookAddedEvent>
    {
        private readonly IBookToCosmosBookService _service;

        public BookAddedHandlerAsync(IBookToCosmosBookService service)
        {
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(object callingEntity, BookAddedEvent domainEvent, Guid uniqueKey)
        {
            await _service.AddCosmosBookAsync(callingEntity as Book);
            return null;
        }
    }
}