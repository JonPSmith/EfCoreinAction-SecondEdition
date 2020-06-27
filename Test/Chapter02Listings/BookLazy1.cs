// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Test.Chapter02Listings
{
    public class BookLazy1
    {
        public BookLazy1() { }                          //#A

        private BookLazy1(ILazyLoader lazyLoader)       //#B
        {                                               //#B
            _lazyLoader = lazyLoader;                   //#B
        }                                               //#B
        private readonly ILazyLoader _lazyLoader;       //#B

        public int Id { get; set; }

        public PriceOffer Promotion { get; set; }       //#C

        private ICollection<Lazy1Review> _reviews;             //#D                  
        public ICollection<Lazy1Review> Reviews                //#E
        {
            get => _lazyLoader.Load(this, ref _reviews);//#F
            set => _reviews = value;                    //#G
        }
    }
    /******************************************************************
    #A You need a public constructor so that you can create this book in your code
    #B This private constructor is used by EF Core to inject the LazyLoader
    #C This is a normal relational link which isn't loaded via lazy loading
    #D The actual reviews are held in a backing field (see section 8.7) 
    #E This is the list that you the developer will access
    #F A read of the property will trigger a lazy loading of the data (if not already loaded)
    #G The set simply updates the backing field
     ***************************************************************/
}