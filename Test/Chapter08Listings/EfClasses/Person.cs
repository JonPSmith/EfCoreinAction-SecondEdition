// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Test.Chapter08Listings.EfClasses
{
    public class Person
    {
        public int PersonId { get; set; }

        public string RealName { get; set; }

        [MaxLength(256)]
        [Required]
        public string Name { get; set; } //#A
        /************************************************
        #A This holds the ASP.NET authorization Claim called "Name", which is the person's email address and is unique 
         * *********************************************/

        //------------------------------
        //relationships

        public ContactInfo ContactInfo { get; set; }

        [InverseProperty(nameof(LibraryBook.Librarian))]   //#A
        public IList<LibraryBook> 
            LibrarianBooks { get; set; }

        [InverseProperty(nameof(LibraryBook.OnLoanTo))]    //#B
        public IList<LibraryBook> 
            BooksBorrowedByMe { get; set; }
    }
    /*********************************************
    A# This links the LibrarianBooks to the Librarian navigational property in the LibraryBook class
    #B This links the BooksBorrowedByMe list to the OnLoanTo navigational property in the LibraryBook class
     * ******************************************/
}