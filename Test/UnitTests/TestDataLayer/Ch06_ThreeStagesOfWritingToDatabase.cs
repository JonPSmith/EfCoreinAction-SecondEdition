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
    public class Ch06_ThreeStagesOfWritingToDatabase
    {
        private readonly ITestOutputHelper _output;

        public Ch06_ThreeStagesOfWritingToDatabase(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestOk()
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

                //STAGE1
                var author = context.Authors.First();
                var bookAuthor = new BookAuthor {Author = author};
                var book = new Book
                {
                    Title = "Test Book", 
                    AuthorsLink = new List<BookAuthor> {bookAuthor}
                };
                //These navigational links have not been set up
                book.AuthorsLink.Single().Book.ShouldBeNull();
                author.BooksLink.ShouldBeNull();
                //tracked PK/FK values not set
                context.Entry(book).Property(nameof(Book.BookId)).CurrentValue.ShouldEqual(0);
                context.Entry(bookAuthor).Property(nameof(BookAuthor.BookId)).CurrentValue.ShouldEqual(0);
                context.Entry(bookAuthor).Property(nameof(BookAuthor.AuthorId)).CurrentValue.ShouldEqual(0);

                //STAGE2
                context.Add(book);
                //Extra Navigational links filled in after call to Add
                book.AuthorsLink.Single().Book.ShouldEqual(book);
                author.BooksLink.Single().ShouldEqual(book.AuthorsLink.Single());
                //Real PKs/FKs
                book.BookId.ShouldEqual(0);
                book.AuthorsLink.Single().BookId.ShouldEqual(0);
                book.AuthorsLink.Single().AuthorId.ShouldEqual(author.AuthorId);
                //tracked PK/FK values of new entities should be negative for Added classes, or actual PK if read in/Unchanged
                var tempBookId = (int) context.Entry(bookAuthor).Property(nameof(BookAuthor.BookId)).CurrentValue;
                (tempBookId < 0).ShouldBeTrue();
                context.Entry(book).Property(nameof(Book.BookId)).CurrentValue.ShouldEqual(tempBookId);
                ((int)context.Entry(bookAuthor).Property(nameof(BookAuthor.AuthorId)).CurrentValue).ShouldEqual(author.AuthorId);

                //STAGE3
                context.SaveChanges();
                book.BookId.ShouldEqual(5);
                book.AuthorsLink.Single().BookId.ShouldEqual(book.BookId);
                book.AuthorsLink.Single().AuthorId.ShouldEqual(author.AuthorId);
            }
        }
    }
}