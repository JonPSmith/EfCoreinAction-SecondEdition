// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.ForHandlers;
using NetCore.AutoRegisterDi;
using StatusGeneric;

namespace BookApp.Infrastructure.Books.CosmosDb.EventsHandlers
{
    [DoNotAutoRegister]
    public class BookDeletedHandlerAsync : IDuringSaveEventHandlerAsync<BookDeleteEvent>
    {
        private readonly IBookToCosmosBookService _service;

        public BookDeletedHandlerAsync(IBookToCosmosBookService service)
        {
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(object callingEntity, BookDeleteEvent domainEvent, Guid uniqueKey)
        {
            await _service.DeleteCosmosBookAsync(((Book)callingEntity).BookId);
            return null;
        }
    }
}