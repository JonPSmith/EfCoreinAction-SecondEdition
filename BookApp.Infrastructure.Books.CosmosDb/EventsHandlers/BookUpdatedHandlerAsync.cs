// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CosmosDb.EventsHandlers
{
    public class BookUpdatedHandlerAsync : IDuringSaveEventHandlerAsync<BookUpdatedEvent>
    {
        private readonly IBookToCosmosBookService _service;

        public BookUpdatedHandlerAsync(IBookToCosmosBookService service)
        {
            _service = service;
        }
        public async Task<IStatusGeneric> HandleAsync(object callingEntity, BookUpdatedEvent domainEvent, Guid uniqueKey)
        {
            await _service.UpdateCosmosBookAsync(callingEntity as Book);
            return null;
        }
    }
}