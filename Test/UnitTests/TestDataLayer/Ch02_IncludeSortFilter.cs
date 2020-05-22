// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public class Ch02_IncludeSortFilter
    {
    private readonly ITestOutputHelper _output;

    public Ch02_IncludeSortFilter(ITestOutputHelper output)
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
    public void TestIncludeSortReviewsDisconnected()
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
                    new Review {NumStars = 2}, new Review {NumStars = 1}, new Review {NumStars = 3}
                }
            };
            context.Add(newBook);
            context.SaveChanges();
        }
        using (var context = new EfCoreContext(options))
        {
            //ATTEMPT
            var query = context.Books
            .Include(x => x.Reviews.OrderBy(y => y.NumStars));
            var books = query.ToList();

            //VERIFY
            _output.WriteLine(query.ToQueryString());
            books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new[] { 1, 2, 3 });
            var hashSet = new HashSet<Review>(context.Set<Review>().ToList());
            hashSet.Select(x => x.NumStars).ShouldEqual(new[] { 2,1,3 });
        }
    }


    [Fact]
    public void TestEagerLoadWithSortFilterAllOk()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using (var context = new EfCoreContext(options))
        {
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            //BUG in EF Core release 5.3 - see https://github.com/dotnet/efcore/issues/20777
            var book = context.Books
            .Include(r => r.AuthorsLink           //#A
                .OrderBy(y => y.Order))           //#A
                    .ThenInclude(r => r.Author)
            .Include(r => r.Reviews               //#B
                .Where(y => y.NumStars == 5))     //#B
            .Include(r => r.Promotion)
            .First();
            /*********************************************************
            #A Sort example: On the eager loading of the AuthorsLink collection you sort the BookAuthors so that the Authors will be in the correct order to display
            #B Filter example: here you only load the Reviews with a start rating of 5
            * *******************************************************/

            //VERIFY
            book.AuthorsLink.ShouldNotBeNull();
            book.AuthorsLink.First()
                .Author.ShouldNotBeNull();

            book.Reviews.ShouldNotBeNull();
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
            var newBook = new Book
            {
                AuthorsLink = new List<BookAuthor>
                {
                    new BookAuthor {Author = new Author {Name = "Author2"}, Order = 2},
                    new BookAuthor {Author = new Author {Name = "Author1"}, Order = 1},
                }
            };
            context.Add(newBook);
            context.SaveChanges();
        }
        using (var context = new EfCoreContext(options))
        {

            //ATTEMPT
            var query = context.Books
            .Include(x => x.AuthorsLink.OrderBy(y => y.Order))
            .ThenInclude(x => x.Author);
            var books = query.ToList();

            //VERIFY
            _output.WriteLine(query.ToQueryString());
            books.Single().AuthorsLink.Select(x => x.Author.Name).ShouldEqual(new[] {"Author1", "Author2" });
        }
    }

    //This isn't supported yet
    //public void TestThenIncludeSortSingle()
    //{
    //    //SETUP
    //    var options = SqliteInMemory.CreateOptions<EfCoreContext>();
    //    using (var context = new EfCoreContext(options))
    //    {
    //        context.Database.EnsureCreated();
    //        var newBook = new Book
    //        {
    //            AuthorsLink = new List<BookAuthor>
    //            {
    //                new BookAuthor {Author = new Author {Name = "Author2"}, Order = 2},
    //                new BookAuthor {Author = new Author {Name = "Author1"}, Order = 1},
    //            }
    //        };
    //        context.Add(newBook);
    //        context.SaveChanges();

    //        //ATTEMPT
    //        //BUG in EF Core release 5.3 - see https://github.com/dotnet/efcore/issues/20777
    //        var query = context.Books
    //            .Include(x => x.AuthorsLink)
    //                .ThenInclude(x => x.Author.OrderBy(y => y.Name));
    //        var books = query.ToList();

    //        //VERIFY
    //        _output.WriteLine(query.ToQueryString());
    //        books.Single().AuthorsLink.Select(x => x.Author.Name).ShouldEqual(new[] { "Author1", "Author2" });
    //    }
    //}

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
            books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new[] {2});
        }
    }


    }
}