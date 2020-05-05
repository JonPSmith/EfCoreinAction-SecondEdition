// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfCode;
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
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books
                    .First(x => x.Reviews.Any());

                //VERIFY
                book.Reviews.ShouldBeNull();
            }
        }

        [Fact]
        public void TestMissingIncludeFailSafeExceptionOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();
            }
            using (var context = new EfCoreContext(options))
            {
                var book = context.Books
                    .First(x => x.Reviews.Any());

                //ATTEMPT
                var ex = Assert.Throws<NullReferenceException>(() => book.Reviews.Count);

                //VERIFY
                
            }
        }

        [Fact]
        public void TestMissingIncludeNotSafeOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                var book = new BookNotSafe();
                book.Reviews.Add(new ReviewNotSafe());
                context.Add(book);
                context.SaveChanges();
            }
            using (var context = new Chapter06Context(options))
            {
                //ATTEMPT
                var book = context.Books   //#A
                    //... missing Include(x => x.Reviews) //#B
                    .First(x => x.Reviews.Any());

                //VERIFY
                book.Reviews.ShouldNotBeNull();
            }
        }


    }
}