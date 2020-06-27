// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using DataLayer.EfClasses;

namespace Test.Chapter02Listings
{
    /// <summary>
    /// This uses Microsoft.EntityFrameworkCore.Proxies and virtual to do lazy loading
    /// </summary>
    public class BookLazyProxy
    {
        public int Id { get; set; }

        //NOTE: all properties need to be virtual
        public virtual PriceOffer Promotion { get; set; }
        public virtual ICollection<LazyReview> Reviews { get; set; }

    }
}