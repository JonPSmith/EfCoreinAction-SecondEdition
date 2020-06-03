// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace Test.Chapter08Listings.EfClasses
{
    public class LibraryBook
    {
        public int LibraryBookId { get; set; }

        public string Title { get; set; }

        //-----------------------------------
        //Relationships

        [MaxLength(256)]
        [Required]
        public int LibrarianUserId { get; set; }

        public Person Librarian { get; set; }

        public int? OnLoanToPersonId { get; set; }
        public Person OnLoanTo { get; set; }
    }
}