// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace BookApp.Domain.Books
{
    public class BookTag
    {
        private BookTag(){} //For EF Core

        public BookTag(Book book, Tag tag)
        {
            Book = book;
            Tag = tag;
        }

        public int BookId { get; private set; }

        [Required]
        [MaxLength(40)]
        public string TagId { get; private set; }

        //-------------------------------------------
        //relationships

        public Book Book { get; private set; }
        public Tag Tag { get; private set; }
    }
}