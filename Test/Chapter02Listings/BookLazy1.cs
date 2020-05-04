// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Test.Chapter02Listings
{
    public class BookLazy1
    {
        public int Id { get; set; }

        public BookLazy1() { }

        private BookLazy1(ILazyLoader lazyLoader)
        {
            _lazyLoader = lazyLoader;
        }

        private readonly ILazyLoader _lazyLoader;
        private ICollection<LazyReview> _reviews;

        //This is here to show how lazy loading works
        public ICollection<LazyReview> Reviews
        {
            get => _lazyLoader.Load(this, ref _reviews);
            set => _reviews = value;
        }
    }
}