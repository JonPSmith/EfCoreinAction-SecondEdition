// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DataLayer.Interfaces;

namespace DataLayer.EfClasses
{
    public class Book : ISoftDelete                   
    {
        public int BookId { get; set; }

        [Required] //#A
        [MaxLength(256)] //#B
        public string Title { get; set; }

        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }

        [MaxLength(64)] //#B
        public string Publisher { get; set; }

        public decimal Price { get; set; }

        [MaxLength(512)] //#B
        public string ImageUrl { get; set; }

        //-----------------------------------------------
        //relationships

        public PriceOffer Promotion { get; set; }
        public ICollection<Review> Reviews { get; set; }

        public ICollection<Tag> Tags { get; set; }

        public ICollection<BookAuthor> 
            AuthorsLink { get; set; }

        public bool SoftDeleted { get; set; }
    }
    /****************************************************
    #A This tells EF Core that the string is non-nullable.
    #B Defines the size of the string column in the database
     * **************************************************/
}