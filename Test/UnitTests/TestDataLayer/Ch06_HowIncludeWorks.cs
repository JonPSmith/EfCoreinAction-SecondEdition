// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_HowIncludeWorks
    {
        private readonly ITestOutputHelper _output;

        public Ch06_HowIncludeWorks(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestBookQueryWithIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var book4Reviews2Authors = EfTestData.CreateDummyBooks(5).Last();
                context.Add(book4Reviews2Authors);
                context.SaveChanges();

                //ATTEMPT
                var query = context.Books
                    .Include(x => x.Reviews)
                    .Include(x => x.AuthorsLink)
                        .ThenInclude(x => x.Author);

                //VERIFY
                _output.WriteLine(query.ToQueryString());
            }
        }

        [Fact]
        public void TestIncludeSortSingle()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var newBook = new Book { Reviews = new List<Review>
                {
                    new Review { NumStars = 2 } , new Review { NumStars = 1 }
                } };
                context.Add(newBook);
                context.SaveChanges();

                //ATTEMPT
                //BUG in EF Core release 5.3 - see https://github.com/dotnet/efcore/issues/20777
                var query = context.Books
                    .Include(x => x.Reviews.OrderByDescending(y => y.NumStars));
                var books = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new []{1,2});
            }
        }

        [Fact]
        public void TestIncludeFilterSingle()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var newBook = new Book
                {
                    Reviews = new List<Review>
                    {
                        new Review {NumStars = 2}, new Review {NumStars = 1}
                    }
                };
                context.Add(newBook);
                context.SaveChanges();
            }
            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var query = context.Books
                    .Include(x => x.Reviews.Where(y => y.NumStars > 1)); 
                var books = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new[] { 2 });
            }
        }


    }
}