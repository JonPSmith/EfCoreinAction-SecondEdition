// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_FailSafeCollections
    {
        public Ch06_FailSafeCollections(ITestOutputHelper output)
        {
            _output = output;
        }

        private readonly ITestOutputHelper _output;

        [Fact]
        public void TestMissingIncludeFailSafeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .First(x => x.Reviews.Any());

            //VERIFY
            book.Reviews.ShouldBeNull();
        }

        [Fact]
        public void TestMissingIncludeFailSafeExceptionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var book = context.Books
                .First(x => x.Reviews.Any());

            //ATTEMPT
            var ex = Assert.Throws<NullReferenceException>(() => book.Reviews.Count);

            //VERIFY
        }

        [Fact]
        public void TestMissingIncludeReplaceListAddsToDatabaseOk()
        {
            //SETUP
            int twoReviewBookId;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            twoReviewBookId = context.SeedDatabaseFourBooks().Last().BookId;

            context.ChangeTracker.Clear();

            var book = context.Books
                //missing .Include(x => x.Reviews)
                .Single(p => p.BookId == twoReviewBookId);

            //ATTEMPT
            book.Reviews = new List<Review>{ new Review{ NumStars = 1}};
            context.SaveChanges();

            //VERIFY
            context.Set<Review>().Count().ShouldEqual(3);
        }

        [Fact]
        public void TestMissingIncludeNotSafeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            var bookSetup = new BookNotSafe();
            bookSetup.Reviews.Add(new ReviewNotSafe());
            context.Add(bookSetup);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books   //#A
                //... missing Include(x => x.Reviews) //#B
                .First(x => x.Reviews.Any());

            //VERIFY
            book.Reviews.ShouldNotBeNull();
        }


    }
}