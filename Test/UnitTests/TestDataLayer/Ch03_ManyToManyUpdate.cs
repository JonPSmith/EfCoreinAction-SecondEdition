// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

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

            context.ChangeTracker.Clear();
            
            //VERIFY
            var bookAgain = context.Books 
                .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)                    
                .Single(p => p.BookId == book.BookId);
            bookAgain.AuthorsLink.ShouldNotBeNull();
            bookAgain.AuthorsLink.Count.ShouldEqual(1);
            bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Martin Fowler");
            context.Authors.Count(p => p.Name == "Future Person").ShouldEqual(1);
        }

        [Fact]
        public void TestAddExtraAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books                           //#A
                .Include(p => p.AuthorsLink)                   //#A
                .Single(p => p.Title == "Quantum Networking"); //#A

            var existingAuthor = context.Authors        //#B         
                .Single(p => p.Name == "Martin Fowler");//#B

            book.AuthorsLink.Add(new BookAuthor  //#C
            {
                Book = book,                           //#D
                Author = existingAuthor,               //#D
                Order = (byte) book.AuthorsLink.Count  //#E
            });
            context.SaveChanges();                     //#F
            /**********************************************************
            #A This finds the book with title "Quantum Networking", whose current author is "Future Person"
            #B You then find an existing author, in this case "Martin Fowler"
            #C You add a new BookAuthor linking entity to the Book's AuthorsLink collection
            #D You fill in the two navigational properties that are in the many-to-many relationship
            #E You set the Order to the old count of AuthorsLink. In this case that will be 1 (the first author has a value of 0)
            #F The SaveChanges will create a new row in the BookAuthor table
            * *******************************************************/

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.AuthorsLink.OrderBy(p => p.Order)).ThenInclude(p => p.Author)
                .Single(p => p.BookId == book.BookId);
            bookAgain.AuthorsLink.ShouldNotBeNull();
            bookAgain.AuthorsLink.Count.ShouldEqual(2);
            bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Future Person");
            bookAgain.AuthorsLink.Last().Author.Name.ShouldEqual("Martin Fowler");
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
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            var book1 = context.Books
                .Include(p => p.AuthorsLink)
                .Single(p => p.Title == "Quantum Networking");

            var newAuthor1 = context.Authors
                .Single(p => p.Name == "Martin Fowler");
            dto = new ChangeAuthorDto
            {
                BookId = book1.BookId,
                AuthorId = book1.AuthorsLink.First().AuthorId,
                NewAuthorId = newAuthor1.AuthorId
            };

            context.ChangeTracker.Clear();

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

            context.ChangeTracker.Clear();

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

        [Fact]
        public void TestRemoveLinkByDeleteToAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseDummyBooks(1);
            var bookId = context.Books.First().BookId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var existingBook = context.Books
                .Include(book => book.AuthorsLink.OrderBy(x => x.Order))
                .Single(book => book.BookId == bookId);

            var linkToRemove = existingBook.AuthorsLink.Last();
            context.Remove(linkToRemove);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                .Single(p => p.BookId == bookId);
            bookAgain.AuthorsLink.ShouldNotBeNull();
            bookAgain.AuthorsLink.Count.ShouldEqual(1);
            bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Author0000");
        }

        [Fact]
        public void TestRemoveLinkByRemovingFromCollectionToAuthorOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseDummyBooks(1);
            var bookId = context.Books.First().BookId;

            context.ChangeTracker.Clear();

            //ATTEMPT
            var existingBook = context.Books
                .Include(book => book.AuthorsLink.OrderBy(x => x.Order))
                .Single(book => book.BookId == bookId);

            var linkToRemove = existingBook.AuthorsLink.Last();
            existingBook.AuthorsLink.Remove(linkToRemove);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                .Single(p => p.BookId == bookId);
            bookAgain.AuthorsLink.ShouldNotBeNull();
            bookAgain.AuthorsLink.Count.ShouldEqual(1);
            bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Author0000");
            context.Set<BookAuthor>().Count().ShouldEqual(1);
        }

        [Fact]
        public void TestChangeAllAuthorsViaForeignKeyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            ChangeAuthorDto dto;
            using var context = new EfCoreContext(options);
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

            context.ChangeTracker.Clear();

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

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.AuthorsLink).ThenInclude(p => p.Author)
                .Single(p => p.BookId == dto.BookId);
            bookAgain.AuthorsLink.ShouldNotBeNull();
            bookAgain.AuthorsLink.Count.ShouldEqual(1);
            bookAgain.AuthorsLink.First().Author.Name.ShouldEqual("Martin Fowler");
        }

        [Fact]
        public void TestAddAuthorViaForeignKeyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            ChangeAuthorDto dto;
            using var context = new EfCoreContext(options);
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

            context.ChangeTracker.Clear();

            //ATTEMPT
            context.Set<BookAuthor>().Add(new BookAuthor    
            {                                               
                BookId = dto.BookId,                        
                AuthorId = dto.NewAuthorId,                 
                Order = 1                                   
            });                                             
            context.SaveChanges();

            context.ChangeTracker.Clear();

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

        //-----------------------------------------
        //Tags

        [Fact]
        public void TestAddExistingTagToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            var tagsCount = context.Tags.Count();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books //#A
                .Include(p => p.Tags) //#A
                .Single(p => p.Title == "Quantum Networking"); //#A

            var existingTag = context.Tags //#B         
                .Single(p => p.TagId == "Editor's Choice"); //#B

            book.Tags.Add(existingTag); //#C
            context.SaveChanges(); //#D
            /**********************************************************
            #A This finds the book with title "Quantum Networking", whose current author is "Future Person"
            #B You then find the Tag called "Editor's Choice" to add this this book
            #C You simply add the Tag to the Books Tags collection
            #D When SaveChanges is called EF Core will create a new row in the hidden BookTags table
            * *******************************************************/

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");
            bookAgain.Tags.Count.ShouldEqual(2);
            bookAgain.Tags.First().TagId.ShouldEqual("Editor's Choice");
            bookAgain.Tags.Last().TagId.ShouldEqual("Quantum Entanglement");
            tagsCount.ShouldEqual(context.Tags.Count());
        }

        [Fact]
        public void TestAddNewTagToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            var tagsCount = context.Tags.Count();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");

            book.Tags.Add(new Tag{TagId = "Unit Test"}); 
            context.SaveChanges();
            
            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");
            bookAgain.Tags.Count.ShouldEqual(2);
            bookAgain.Tags.First().TagId.ShouldEqual("Quantum Entanglement");
            bookAgain.Tags.Last().TagId.ShouldEqual("Unit Test");
            tagsCount.ShouldEqual(context.Tags.Count()-1);
        }

        [Fact]
        public void TestRemoveTagToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .Include(p => p.Tags)
                .First();

            var tagToRemove = book.Tags.Single(x => x.TagId == "Editor's Choice");
            book.Tags.Remove(tagToRemove);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.Tags)
                .First();
            bookAgain.Tags.Count.ShouldEqual(1);
            bookAgain.Tags.Single().TagId.ShouldEqual("Refactoring");
        }

        [Fact]
        public void TestAddDuplicateExistingTagToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");

            book.Tags.Add(book.Tags.First());
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var bookAgain = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");
            bookAgain.Tags.Count.ShouldEqual(1);
            bookAgain.Tags.Single().TagId.ShouldEqual("Quantum Entanglement");
        }

        [Fact]
        public void TestAddDuplicateNewTagToBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();
            var tagsCount = context.Tags.Count();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var book = context.Books
                .Include(p => p.Tags)
                .Single(p => p.Title == "Quantum Networking");

            book.Tags.Add( new Tag{TagId =  book.Tags.First().TagId});
            var ex = Assert.Throws<InvalidOperationException>(() =>  context.SaveChanges());

            //VERIFY
            ex.Message.ShouldStartWith("The instance of entity type 'Tag' cannot be tracked because another instance with the key value");
        }

    }
}