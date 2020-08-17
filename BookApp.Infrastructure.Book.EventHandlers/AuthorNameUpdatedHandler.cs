// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Domain.Books.DomainEvents;
using BookApp.Persistence.EfCoreSql.Books;
using GenericEventRunner.DomainParts;
using GenericEventRunner.ForHandlers;
using StatusGeneric;

namespace BookApp.Infrastructure.Book.EventHandlers
{
    public class AuthorNameUpdatedHandler : IBeforeSaveEventHandler<AuthorNameUpdatedEvent>
    {
        private readonly BookDbContext _context;

        public AuthorNameUpdatedHandler(BookDbContext context)
        {
            _context = context;
        }

        public IStatusGeneric Handle(EntityEventsBase callingEntity, AuthorNameUpdatedEvent domainEvent)
        {
            //We go through all the books that have this author as one of its authors
            foreach (var bookWithEvents in _context.Set<BookAuthor>()
                .Where(x => x.AuthorId == domainEvent.ChangedAuthor.AuthorId)
                .Select(x => x.Book))
            {
                //For each book that has this author has its AuthorsOrdered string recomputed.
                var allAuthorsInOrder = _context.Books
                    .Where(x => x.BookId == bookWithEvents.BookId)
                    .Select(x => x.AuthorsLink.OrderBy(y => y.Order).Select(y => y.Author).ToList())
                    .Single();

                //The database hasn't been updated yet, so we have to manually insert the new name into the correct point in the authorsOrdered
                var newAuthorsOrdered = string.Join(", ", allAuthorsInOrder.Select(x =>
                    x.AuthorId == domainEvent.ChangedAuthor.AuthorId
                        ? domainEvent.ChangedAuthor.Name 
                        : x.Name));

                bookWithEvents.AuthorsOrdered = newAuthorsOrdered;
            }

            return null;
        }

    }
}