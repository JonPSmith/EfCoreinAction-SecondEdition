// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BookApp.Domain.Books.DomainEvents;
using GenericEventRunner.DomainParts;

namespace BookApp.Domain.Books
{
    public class Author : EntityEventsBase
    {
        public const int NameLength = 100;
        public const int EmailLength = 100;

        private string _name;
        private HashSet<BookAuthor> _booksLink;

        public Author(string name, string email)
        {
            _name = name;
            Email = email;
        }

        public int AuthorId { get;  private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(NameLength)]
        public string Name
        {
            get => _name;
            private set
            {
                if (value != _name)
                    AddEvent(new AuthorNameUpdatedEvent(this));
                _name = value;
            }
        }

        [MaxLength(EmailLength)]
        public string Email { get; private set; }

        //------------------------------
        //Relationships

        public ICollection<BookAuthor> BooksLink => _booksLink?.ToList();

        public void ChangeName(string newAuthorName)
        {
            Name = newAuthorName;
        }
    }

}