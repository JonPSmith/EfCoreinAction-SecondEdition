// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Book.DomainEvents
{
    public class AuthorNameUpdatedEvent : IDomainEvent
    {
        public AuthorNameUpdatedEvent(Author changedAuthor)
        {
            ChangedAuthor = changedAuthor;
        }

        public Author ChangedAuthor { get; }
    }
}