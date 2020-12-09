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
        public void TestCreateBookWithExistingAuthor_CheckThreeStagesOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //STAGE1
            var author = context.Authors.First();
            var bookAuthor = new BookAuthor {Author = author};
            var book = new Book
            {
                Title = "Test Book", 
                AuthorsLink = new List<BookAuthor> {bookAuthor}
            };
            //Read the states 
            context.Entry(author).State.ShouldEqual(EntityState.Unchanged);
            context.Entry(bookAuthor).State.ShouldEqual(EntityState.Detached);
            context.Entry(book).State.ShouldEqual(EntityState.Detached);
            //These navigational links have not been set up
            book.AuthorsLink.Single().Book.ShouldBeNull();
            author.BooksLink.ShouldBeNull();
            //tracked PK/FK values not set
            context.Entry(book).Property(nameof(Book.BookId)).CurrentValue.ShouldEqual(0);
            context.Entry(bookAuthor).Property(nameof(BookAuthor.BookId)).CurrentValue.ShouldEqual(0);
            context.Entry(bookAuthor).Property(nameof(BookAuthor.AuthorId)).CurrentValue.ShouldEqual(0);

            //STAGE2
            context.Add(book);
            //Read the states 
            context.Entry(author).State.ShouldEqual(EntityState.Unchanged);
            context.Entry(bookAuthor).State.ShouldEqual(EntityState.Added);
            context.Entry(book).State.ShouldEqual(EntityState.Added);
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
            context.Entry(author).State.ShouldEqual(EntityState.Unchanged);
            context.Entry(bookAuthor).State.ShouldEqual(EntityState.Unchanged);
            context.Entry(book).State.ShouldEqual(EntityState.Unchanged);
            book.BookId.ShouldEqual(5);
            book.AuthorsLink.Single().BookId.ShouldEqual(book.BookId);
            book.AuthorsLink.Single().AuthorId.ShouldEqual(author.AuthorId);
        }

        [Fact]
        public void TestTestCreateBookWithExistingAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //STAGE1                                             //#A
            var author = context.Authors.First();                //#B
            var bookAuthor = new BookAuthor { Author = author }; //#C
            var book = new Book                                  //#D
            {                                                    //#D
                Title = "Test Book",                             //#D
                AuthorsLink = new List<BookAuthor> { bookAuthor }//#D
            };                                                   //#D

            //STAGE2
            context.Add(book);                                   //#E

            //STAGE3
            context.SaveChanges();                               //#F
            /*********************************************************
                #A Each of the three stages start with a comment 
                #B This reads in an existing Author for the new book
                #C This creates a new BookAuthor linking table ready to link to Book to the Author
                #D This creates a Book, and fills in the AuthorsLink navigational property with a single entry, linking it to the existing Author
                #E This is where you call the Add method, which tells EF Core that the Book needs to be added to the database. 
                #F SaveChanges now looks the all the tracked entities and works out how to update the database to achieve what you have asked it to do
                 ******************************************************/
        }
    }
}