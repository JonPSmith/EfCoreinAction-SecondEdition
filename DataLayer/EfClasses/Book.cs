// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DataLayer.EfClasses
{
    public class Book //#A
    {
        public int BookId { get; set; } //#B
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }

        /// <summary>
        ///     Holds the url to get the image of the book
        /// </summary>
        public string ImageUrl { get; set; }

        public bool SoftDeleted { get; set; }

        //-----------------------------------------------
        //relationships

        public PriceOffer Promotion { get; set; } //#C
        public ICollection<Review> Reviews { get; set; } //#D

        public ICollection<BookAuthor>
            AuthorsLink { get; set; } //#E


        //---------------------------------------------------
        //Lazy loading parts

        public Book() { }

        private Book(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        private readonly ILazyLoader _lazyLoader;
        private ICollection<LazyReview> _lazyReviews;

        //This is here to show how lazy loading works
        public ICollection<LazyReview> LazyReviews
        {
            get => _lazyLoader.Load(this, ref _lazyReviews);
            set => _lazyReviews = value;
        }
    }

    /****************************************************#
    #A The Book class contains the main book information
    #B I use EF Core's 'by convention' approach to defining the primary key of this entity class. In this case I use <ClassName>Id, and because the property if of type int EF Core assumes that the database will use the SQL IDENTITY command to create a unique key when a new row is added
    #C This is the link to the optional PriceOffer
    #D There can be zero to many Reviews of the book
    #E This provides a link to the Many-to-Many linking table that links to the Authors of this book
     * **************************************************/
}