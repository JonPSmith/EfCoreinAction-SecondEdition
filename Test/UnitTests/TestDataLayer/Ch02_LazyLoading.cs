// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using Microsoft.EntityFrameworkCore;
using Test.Chapter02Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch02_LazyLoading
    {
        private readonly ITestOutputHelper _output;

        public Ch02_LazyLoading(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLazyLoadBookAndReviewUsingILazyLoaderOk()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<LazyInjectContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using var context = new LazyInjectContext(options);
            context.Database.EnsureCreated();
            var bookSetup = new BookLazy1
            {
                Promotion = new PriceOffer{ NewPrice = 5},
                Reviews = new List<Lazy1Review>
                {
                    new Lazy1Review {NumStars = 5}, new Lazy1Review {NumStars = 1}
                }
            };
            context.Add(bookSetup);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showlog = true;
            var book = context.BookLazy1s.Single(); //#A
            var reviews = book.Reviews.ToList(); //#B

            //VERIFY
            book.Promotion.ShouldBeNull();
            context.BookLazy1s.Select(x => x.Promotion).ShouldNotBeNull();
            reviews.Count.ShouldEqual(2);
        }

        [Fact]
        public void TestLazyLoadBookAndReviewUsingActionOk()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<LazyInjectContext>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using var context = new LazyInjectContext(options);
            context.Database.EnsureCreated();
            var bookSetup = new BookLazy2
            {
                Promotion = new PriceOffer { NewPrice = 5 },
                Reviews = new List<Lazy2Review>
                {
                    new Lazy2Review {NumStars = 5}, new Lazy2Review {NumStars = 1}
                }
            };
            context.Add(bookSetup);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showlog = true;
            var book = context.BookLazy2s.Single();
            var reviews = book.Reviews.ToList();

            //VERIFY
            book.Promotion.ShouldBeNull();
            context.BookLazy2s.Select(x => x.Promotion).ShouldNotBeNull();
            reviews.Count.ShouldEqual(2);
        }

        [Fact]
        public void TestLazyLoadBookAndReviewUsingProxiesPackageOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<LazyProxyContext>(
                builder => builder.UseLazyLoadingProxies());
            using var context = new LazyProxyContext(options);
            context.Database.EnsureCreated();
            var bookSetup = new BookLazyProxy
            {
                Reviews = new List<LazyReview>
                {
                    new LazyReview {NumStars = 5}, new LazyReview {NumStars = 1}
                }
            };
            context.Add(bookSetup);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books.Single(); //#A
            book.Reviews.Count().ShouldEqual(2); //#B
            /*********************************************************
                #A We just load the book class
                #B When the Reviews are read, then EF Core will read in the reviews
                * *******************************************************/
        }

        [Fact]
        public void TestLazyLoadCompareIncludeOk()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<LazyProxyContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            }, builder: builder => builder.UseLazyLoadingProxies());

            using var context = new LazyProxyContext(options);
            context.Database.EnsureCreated();
            var book = new BookLazyProxy
            {
                Reviews = new List<LazyReview>
                {
                    new LazyReview {NumStars = 5}, new LazyReview {NumStars = 1}
                }
            };
            context.Add(book);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            showLog = true;
            var book1 = context.Books.TagWith("lazy").Single(); 
            book1.Reviews.Count().ShouldEqual(2);
            var book2 = context.Books.TagWith("include").Include(x => x.Reviews).Single();
            book2.Reviews.Count().ShouldEqual(2);
        }
    }
}