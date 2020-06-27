// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DataLayer.EfClasses;

namespace Test.Chapter02Listings
{
    public class BookLazy2
    {
        public BookLazy2() { }                          

        private BookLazy2(Action<object, string> lazyLoader)   
        {
            LazyLoader = lazyLoader; 
        }
        private Action<object, string> LazyLoader { get; set; }


        public int Id { get; set; }

        public PriceOffer Promotion { get; set; } 

        private ICollection<Lazy2Review> _reviews;                   
        public ICollection<Lazy2Review> Reviews 
        {
            get => LazyLoader.Load(this, ref _reviews);
            set => _reviews = value;  
        }
    }

    public static class PocoLoadingExtensions
    {
        public static TRelated Load<TRelated>(
            this Action<object, string> loader,
            object entity,
            ref TRelated navigationField,
            [CallerMemberName] string navigationName = null)
            where TRelated : class
        {
            loader?.Invoke(entity, navigationName);

            return navigationField;
        }
    }
}