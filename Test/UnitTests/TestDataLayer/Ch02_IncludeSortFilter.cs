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
    public class Ch02_IncludeSortFilter
    {
    private readonly ITestOutputHelper _output;

    public Ch02_IncludeSortFilter(ITestOutputHelper output)
    {
        _output = output;
    }

    //see https://github.com/dotnet/efcore/issues/20777#issuecomment-632123707 and EF Core code
    [Fact]
    public void TestILinkWillPassEfCoreTests()
    {
        //This runs a version of the code in EF Core that decides whether to use the navigational type 
        //see https://github.com/dotnet/efcore/blob/e6674cdbc8e45ffd890ba20441230fb383087e3a/src/EFCore/Metadata/Internal/CollectionTypeFactory.cs#L26-L33
        //var listOfT = typeof(List<>).MakeGenericType(elementType);
        //return collectionType.IsAssignableFrom(listOfT) ? listOfT : null;

        var listOfT = typeof(List<Review>);

        typeof(IList<Review>).IsAssignableFrom(listOfT).ShouldBeTrue();
        typeof(ICollection<Review>).IsAssignableFrom(listOfT).ShouldBeTrue();
    }


    [Fact]
    public void TestBookQueryWithIncludes()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

    //See first bullet point for why the sort doesn't work https://github.com/dotnet/efcore/issues/20777#issuecomment-632101694
    [Fact]
    public void TestIncludeSortReviews()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

        //ATTEMPT
        var query = context.Books
            .Include(x => x.Reviews.OrderBy(y => y.NumStars));
        var books = query.ToList();

        //VERIFY
        _output.WriteLine(query.ToQueryString());
        books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new[] { 2,1,3 }); //WRONG! See comment on test
    }

    [Fact]
    public void TestIncludeSortReviewsDisconnected()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

        context.ChangeTracker.Clear();

        //ATTEMPT
        var query = context.Books
            .Include(x => x.Reviews.OrderBy(y => y.NumStars));
        var books = query.ToList();

        //VERIFY
        _output.WriteLine(query.ToQueryString());
        books.Single().Reviews.Select(x => x.NumStars).ShouldEqual(new[] { 1, 2, 3 });
        var hashSet = new HashSet<Review>(context.Set<Review>().ToList());
        hashSet.Select(x => x.NumStars).ShouldEqual(new[] { 2, 1, 3 });
    }


        [Fact]
    public void TestEagerLoadWithSortFilterAllOk()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
        context.Database.EnsureCreated();
        context.SeedDatabaseFourBooks();

        //ATTEMPT
        var firstBook = context.Books
            .Include(book => book.AuthorsLink //#A
                .OrderBy(bookAuthor => bookAuthor.Order)) //#A
            .ThenInclude(bookAuthor => bookAuthor.Author)
            .Include(book => book.Reviews //#B
                .Where(review => review.NumStars == 5)) //#B
            .Include(book => book.Promotion)
            .First();
        /*********************************************************
            #A Sort example: On the eager loading of the AuthorsLink collection you sort the BookAuthors so that the Authors will be in the correct order to display
            #B Filter example: here you only load the Reviews with a start rating of 5
            * *******************************************************/

        //VERIFY
        firstBook.AuthorsLink.ShouldNotBeNull();
        firstBook.AuthorsLink.First()
            .Author.ShouldNotBeNull();

        firstBook.Reviews.ShouldNotBeNull();
    }

    [Fact]
    public void TestIncludeSortSingle()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

        context.ChangeTracker.Clear();

        //ATTEMPT
        var query = context.Books
            .Include(x => x.AuthorsLink.OrderBy(y => y.Order))
            .ThenInclude(x => x.Author);
        var books = query.ToList();

        //VERIFY
        _output.WriteLine(query.ToQueryString());
        books.Single().AuthorsLink.Select(x => x.Author.Name).ShouldEqual(new[] {"Author1", "Author2" });
    }

    [Fact]
    public void TestThenIncludeSortSingle()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

        //ATTEMPT
        var query = context.Books
            .Include(book => book.AuthorsLink)//.OrderBy(y => y.Order))
            .ThenInclude(bookAuthor => bookAuthor.OrderBy(y => y.Author.Name));
        var books = query.ToList();

        //VERIFY
        _output.WriteLine(query.ToQueryString());
        books.Single().AuthorsLink.Select(x => x.Author.Name).ShouldEqual(new[] { "Author1", "Author2" });
    }

    [Fact]
    public void TestIncludeFilterSingle()
    {
        //SETUP
        var options = SqliteInMemory.CreateOptions<EfCoreContext>();
        using var context = new EfCoreContext(options);
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

        context.ChangeTracker.Clear();

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