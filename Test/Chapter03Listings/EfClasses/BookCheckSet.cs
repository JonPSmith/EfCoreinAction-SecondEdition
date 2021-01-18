// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Test.Chapter03Listings.EfClasses
{
    public class BookCheckSet
    {
        public BookCheckSet()
        {
            Reviews = new List<ReviewSetCheck>();
            Tags = new List<TagCheckSet>();
            AuthorsLink = new List<BookAuthorCheckSet>();
        }
        
        [Key]
        public int BookId { get; set; } 
        public string Title { get; set; }

        //-----------------------------------------------
        //relationships

        public ICollection<ReviewSetCheck> Reviews { get; set; } 

        public ICollection<TagCheckSet> Tags { get; set; } 

        public ICollection<BookAuthorCheckSet> AuthorsLink { get; set; } 
    }
}