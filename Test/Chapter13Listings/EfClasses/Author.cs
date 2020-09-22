// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Test.Chapter13Listings.EfClasses
{
    public class Author
    {
        public const int NameLength = 100;
        public const int EmailLength = 100;

        private string _name;
        private HashSet<BookApp.Domain.Books.BookAuthor> _booksLink;

        public Author(string name, string email)
        {
            _name = name;
            Email = email;
        }

        public int AuthorId { get;  private set; }

        [Required(AllowEmptyStrings = false)]
        [MaxLength(NameLength)]
        public string Name { get; private set; }

        [MaxLength(EmailLength)]
        public string Email { get; private set; }

        //------------------------------
        //Relationships

        public ICollection<BookApp.Domain.Books.BookAuthor> BooksLink => _booksLink?.ToList();

        public void ChangeName(string newAuthorName)
        {
            Name = newAuthorName;
        }
    }

}