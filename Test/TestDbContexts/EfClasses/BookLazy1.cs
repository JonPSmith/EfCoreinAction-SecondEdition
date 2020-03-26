// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Test.TestDbContexts.EfClasses
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
        private ICollection<LazyReview> _lazyReviews;

        //This is here to show how lazy loading works
        public ICollection<LazyReview> LazyReviews
        {
            get => _lazyLoader.Load(this, ref _lazyReviews);
            set => _lazyReviews = value;
        }
    }
}