// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericEventRunner.ForHandlers;
using StatusGeneric;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using NetCore.AutoRegisterDi;

namespace BookApp.Infrastructure.Books.CosmosDb.EventsHandlers
{
    [DoNotAutoRegister]
    public class AuthorNameUpdatedHandlerAsync : IDuringSaveEventHandlerAsync<AuthorNameUpdatedEvent>
    {
        private readonly BookDbContext _sqlContext;
        private readonly IBookToCosmosBookService _service;

        public AuthorNameUpdatedHandlerAsync(BookDbContext sqlContext, IBookToCosmosBookService service)
        {
            _sqlContext = sqlContext;
            _service = service;
        }

        public async Task<IStatusGeneric> HandleAsync(object callingEntity, AuthorNameUpdatedEvent domainEvent, Guid uniqueKey)
        {

            var bookIds = await _sqlContext.Authors
                .Where(x => x.AuthorId == ((Author) callingEntity).AuthorId)
                .SelectMany(x => x.BooksLink.Select(y => y.BookId)).ToListAsync();

            await _service.UpdateManyCosmosBookAsync(bookIds);
            return null;
        }
    }
}