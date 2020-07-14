// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Test.Chapter08Listings.EfClasses;
using Test.Chapter08Listings.EFCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch08_InverseProperty
    {
        private readonly ITestOutputHelper _output;

        public Ch08_InverseProperty(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLibraryBookLibrarianAndOnLoadOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var librarian = new Person { Name = "Librarian", Email = "librarian@somewhere.com" };
                var reader = new Person { Name = "OnLoanTo", Email = "reader@somewhere.com" };
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action",
                    Librarian = librarian,
                    OnLoanTo = reader
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.LibraryBooks.Count().ShouldEqual(1);
                context.People.Count().ShouldEqual(2);
            }
        }

        [Fact]
        public void TestLibraryBookLibrarianOnlyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var librarian = new Person {Name = "Librarian", Email = "libarian@somewhere.com"};
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action",
                    Librarian = librarian
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.LibraryBooks.Count().ShouldEqual(1);
                context.People.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestLibraryBookNoLibrarianOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter08DbContext>();
            using (var context = new Chapter08DbContext(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var book = new LibraryBook
                {
                    Title = "Entity Framework in Action"
                };
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.LibraryBooks.Count().ShouldEqual(1);
            }
        }
    }
}