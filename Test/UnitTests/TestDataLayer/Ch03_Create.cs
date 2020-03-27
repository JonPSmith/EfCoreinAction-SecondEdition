// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_Create
    {
        [Fact]
        public void TestCreateBookWithReview()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var book = new Book                     //#A
                {                                       //#A
                    Title = "Test Book",                //#A
                    PublishedOn = DateTime.Today,       //#A
                    Reviews = new List<Review>()        //#B
                    {
                        new Review                       //#C
                        {                                //#C
                            NumStars = 5,                //#C
                            Comment = "Great test book!",//#C
                            VoterName = "Mr U Test"      //#C
                        }
                    }
                };

                //ATTEMPT
                context.Add(book);                      //#D
                context.SaveChanges();                  //#E
                /******************************************************
                #A This creates the book with the title "Test Book"
                #B I create a new collection of Reviews
                #C I add one review, with its content
                #D It uses the .Add method to add the book to the application's DbContext property, Books
                #E It calls the SaveChanges() method from the application's DbContext to update the database. It finds a new Book, which has a collection containing a new Review, so it adds both of these to the database
                 * *****************************************************/

                //VERIFY
                context.Books.Count().ShouldEqual(1);
                context.Set<Review>().Count().ShouldEqual(1); 
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorSavedToDatabase()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var oneBook = 
                    EfTestData.CreateDummyBookOneAuthor();//#A
                context.Add(oneBook);               //#A
                context.SaveChanges();                    //#A

                var book = new Book                     //#B
                {                                       //#B
                    Title = "Test Book",                //#B
                    PublishedOn = DateTime.Today        //#B
                };                                      //#B
                book.AuthorsLink = new List<BookAuthor> //#C
                {                                       //#C
                    new BookAuthor                      //#C
                    {                                   //#C
                        Book = book,                    //#C
                        Author = oneBook.AuthorsLink    //#C
                             .First().Author            //#C
                    }                                   //#C
                };                                      //#C

                //ATTEMPT
                context.Add(book);                //#D
                context.SaveChanges();                  //#D
                /************************************************************
                #A This method creates dummy books for testing. I create one dummy book with one Author and add it to the empty database
                #B This creates a book in the same way as the previous example, but sets up its Author
                #C This adds a AuthorBook linking entry, but it reads in an existing the Author from the first book
                #D This is the same process: add the new book to the DbContext Books property and call SaveChanges
                 * *********************************************************/

                //VERIFY
                context.Books.Count().ShouldEqual(2);   //#E
                context.Authors.Count().ShouldEqual(1); //#F
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorAddedButNotSavedToDatabase()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();

                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();
                context.Add(oneBook);               

                var book = new Book                    
                {                                      
                    Title = "Test Book",               
                    PublishedOn = DateTime.Today       
                };                                     
                book.AuthorsLink = new List<BookAuthor>
                {                                      
                    new BookAuthor                     
                    {                                  
                        Book = book,                   
                        Author = oneBook.AuthorsLink   
                             .First().Author           
                    }                                  
                };                                     

                //ATTEMPT
                context.Add(book);               
                context.SaveChanges();                 

                //VERIFY
                context.Books.Count().ShouldEqual(2);
                context.Authors.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateBookWithExistingAuthorAttached()
        {
            //SETUP
            Author disconnectedAuthor;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                disconnectedAuthor = context.Authors.First();
            }

            using (var context = new EfCoreContext(options))
            {
                var book = new Book
                {
                    Title = "Test Book",
                    PublishedOn = DateTime.Today
                };
                context.Authors.Attach(disconnectedAuthor);
                book.AuthorsLink = new List<BookAuthor>
                {
                    new BookAuthor
                    {
                        Book = book,
                        Author = disconnectedAuthor
                    }
                };

                //ATTEMPT
                context.Add(book);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(5);
                context.Authors.Count().ShouldEqual(3);
            }
        }

        [Fact]
        public void TestCreateBookAddTwice()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Add(oneBook);
                context.Add(oneBook);
                context.SaveChanges();

                //VERIFY
                context.Books.Count().ShouldEqual(1);
            }
        }

        [Fact]
        public void TestCreateBookWriteTwiceBad()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                var oneBook =
                    EfTestData.CreateDummyBookOneAuthor();

                //ATTEMPT
                context.Add(oneBook);
                context.SaveChanges();
                var state1 = context.Entry(oneBook).State;
                context.Add(oneBook);
                var state2 = context.Entry(oneBook).State;
                var ex = Assert.Throws<DbUpdateException>( () => context.SaveChanges());

                //VERIFY
                ex.Message.ShouldEqual("An error occurred while updating the entries. See the inner exception for details.");
                state1.ShouldEqual(EntityState.Unchanged);
                state2.ShouldEqual(EntityState.Added);
            }
        }
    }
}