// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using DataLayer.EfClasses;

namespace Test.Chapter02Listings
{
    public class BookHashReview
    {
        public int BookId { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime PublishedOn { get; set; }
        public string Publisher { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }

        public bool SoftDeleted { get; set; }

        //-----------------------------------------------
        //relationships

        public HashSet<Review> Reviews { get; set; }
    }
}