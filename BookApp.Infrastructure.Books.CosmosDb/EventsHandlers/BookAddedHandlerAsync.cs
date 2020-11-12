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
    public class BookAddedHandlerAsync : IDuringSaveEventHandlerAsync<BookAddedEvent>
    {
        private readonly IBookToCosmosBookService _service;

        public BookAddedHandlerAsync(IBookToCosmosBookService service)
        {
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(object callingEntity, BookAddedEvent domainEvent, Guid uniqueKey)
        {
            var bookId = ((Book) callingEntity).BookId;
            await _service.AddCosmosBookAsync(bookId);
            return null;
        }
    }
}