// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

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
    public class Ch03_ManyToManyUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_ManyToManyUpdate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestChangeAuthorsNewListOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books                          //#A
                    .Include(p => p.AuthorsLink)                  //#A
                    .Single(p => p.Title == "Quantum Networking");//#A

                var newAuthor = context.Authors                   //#B
                    .Single(p => p.Name == "Martin Fowler");      //#B

                book.AuthorsLink = new List<BookAuthor>           //#C
                {                                                 //#C
                    new BookAuthor                                //#C
                    {                                             //#C
                        Book = book,                              //#C
                        Author = newAuthor,                       //#C
                        Order = 0                                 //#C
                    }                                             //#C
                };                                                //#C

                context.SaveChanges();                            //#D          
                /**********************************************************
                #A This finds the book with title "Quantum Networking", whose current author is "Future Person"
                #B I then find an existing author, in this case "Martin Fowler"
                #C Then I completely replace the list of authors, so that the "Quantum Networking" book's author is "Martin Fowler"
                #C The SaveChanges method calls DetectChanges, which finds that the AuthorsLink has changes so deletes the old ones and replaces it with the new link
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books 
                    .Include(p => p.AuthorsLink)                    
                    .Single(p => p.BookId == book.BookId);
                bookAgain.AuthorsLink.ShouldNotBeNull();
                bookAgain.AuthorsLink.Count.ShouldEqual(1);
                bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Martin Fowler");
                context.Authors.Count(p => p.Name == "Future Person").ShouldEqual(1);
            }
        }

        [Fact]
        public void TestAddExtraAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books                          
                    .Include(p => p.AuthorsLink)                  
                    .Single(p => p.Title == "Quantum Networking");

                var newAuthor = context.Authors                   
                    .Single(p => p.Name == "Martin Fowler");

                book.AuthorsLink.Add(new BookAuthor
                {
                    Book = book,
                    Author = newAuthor,
                    Order = (byte) book.AuthorsLink.Count
                });
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.AuthorsLink)
                    .Single(p => p.BookId == book.BookId);
                bookAgain.AuthorsLink.ShouldNotBeNull();
                bookAgain.AuthorsLink.Count.ShouldEqual(2);
                var authorsInOrder = bookAgain.AuthorsLink.OrderBy(p => p.Order).ToList();
                authorsInOrder.First().Author.Name.ShouldEqual("Future Person");
                authorsInOrder.Last().Author.Name.ShouldEqual("Martin Fowler");
            }
        }

        private class ChangeAuthorDto
        {
            public int BookId { get; set; }
            public int AuthorId { get; set; }
            public int NewAuthorId { get; set; }
        }

        [Fact]
        public void TestAddAuthorDisconnectedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            ChangeAuthorDto dto;
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.AuthorsLink)
                    .Single(p => p.Title == "Quantum Networking");

                var newAuthor = context.Authors
                    .Single(p => p.Name == "Martin Fowler");
                dto = new ChangeAuthorDto
                {
                    BookId = book.BookId,
                    AuthorId = book.AuthorsLink.First().AuthorId,
                    NewAuthorId = newAuthor.AuthorId
                };
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var book = context.Books
                    .Include(p => p.AuthorsLink)
                    .Single(p => p.BookId == dto.BookId);
                var newAuthor = context.Find<Author>(dto.NewAuthorId);

                book.AuthorsLink.Add(new BookAuthor
                {
                    Book = book,
                    Author = newAuthor,
                    Order = (byte)book.AuthorsLink.Count
                });
                context.SaveChanges();

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                    .Single(p => p.BookId == dto.BookId);
                bookAgain.AuthorsLink.ShouldNotBeNull();
                bookAgain.AuthorsLink.Count.ShouldEqual(2);
                var authorsInOrder = bookAgain.AuthorsLink.OrderBy(p => p.Order).ToList();
                authorsInOrder.First().Author.Name.ShouldEqual("Future Person");
                authorsInOrder.Last().Author.Name.ShouldEqual("Martin Fowler");
            }
        }

        [Fact]
        public void TestChangeAllAuthorsViaForeignKeyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            ChangeAuthorDto dto;
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.AuthorsLink)
                    .Single(p => p.Title == "Quantum Networking");

                var newAuthor = context.Authors
                    .Single(p => p.Name == "Martin Fowler");
                dto = new ChangeAuthorDto
                {
                    BookId = book.BookId,
                    AuthorId = book.AuthorsLink.First().AuthorId,
                    NewAuthorId = newAuthor.AuthorId
                };
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                var orgBookAuthor = context    //#A
                    .Find<BookAuthor>(dto.BookId, dto.AuthorId); //#A
                context.Set<BookAuthor>().Remove(orgBookAuthor); //#B
                context.Set<BookAuthor>().Add(new BookAuthor     //#C
                {                                                //#C
                    BookId = dto.BookId,                         //#C
                    AuthorId = dto.NewAuthorId,                  //#C
                    Order = 0                                    //#C
                });                                              //#C
                context.SaveChanges();                           //#D
                /*****************************************************
                #A I find the existing BookAuthor link using the BookId and the Authorid of the original author
                #B I then delete the original link
                #C Now I create a new BookAuthor link to the Author chosen by the user and add it the BookAuthor table
                #D Finally I call SaveChanges which find the deleted BookAuthor entry and the new BookAuthor entry and deletes/adds then respectively
                 * **************************************************/        

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                    .Single(p => p.BookId == dto.BookId);
                bookAgain.AuthorsLink.ShouldNotBeNull();
                bookAgain.AuthorsLink.Count.ShouldEqual(1);
                bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Martin Fowler");
            }
        }

        [Fact]
        public void TestAddAuthorViaForeignKeyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            ChangeAuthorDto dto;
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var book = context.Books
                    .Include(p => p.AuthorsLink)
                    .Single(p => p.Title == "Quantum Networking");

                var newAuthor = context.Authors
                    .Single(p => p.Name == "Martin Fowler");
                dto = new ChangeAuthorDto
                {
                    BookId = book.BookId,
                    NewAuthorId = newAuthor.AuthorId
                };
            }

            using (var context = new EfCoreContext(options))
            {
                //ATTEMPT
                context.Set<BookAuthor>().Add(new BookAuthor    
                {                                               
                    BookId = dto.BookId,                        
                    AuthorId = dto.NewAuthorId,                 
                    Order = 1                                   
                });                                             
                context.SaveChanges();                          

                //VERIFY
                var bookAgain = context.Books
                    .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                    .Single(p => p.BookId == dto.BookId);
                bookAgain.AuthorsLink.ShouldNotBeNull();
                bookAgain.AuthorsLink.Count.ShouldEqual(2);
                var authorsInOrder = bookAgain.AuthorsLink.OrderBy(p => p.Order).ToList();
                authorsInOrder.First().Author.Name.ShouldEqual("Future Person");
                authorsInOrder.Last().Author.Name.ShouldEqual("Martin Fowler");
            }
        }

    }
}