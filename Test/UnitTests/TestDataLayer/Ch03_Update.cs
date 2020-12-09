// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServiceLayer.AdminServices;
using ServiceLayer.AdminServices.Concrete;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_Update
    {
        private readonly ITestOutputHelper _output;

        public Ch03_Update(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void UpdatePublicationDate()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var book = context.Books                          //#A
                .Single(p => p.Title == "Quantum Networking");//#B
            book.PublishedOn = new DateTime(2058, 1, 1);      //#C     
            context.SaveChanges();                            //#D
            /**********************************************************
                #A Finds the specific book we want to update. In the case our special book on Quantum Networking
                #B Single means the query will fail if there is no book of that name, or many books with that name
                #C Changes the expected publication date to year 2058 (it was 2057)
                #D Calls SaveChanges which includes running a method called DetectChanges. This spots that the PublishedOn property has been changed
                * *******************************************************/

            //VERIFY
            var bookAgain = context.Books                     //#E
                .Single(p => p.Title == "Quantum Networking");//#E
            bookAgain.PublishedOn                             //#F
                .ShouldEqual(new DateTime(2058, 1, 1));       //#F
        }
        /**********************************************************
        #E This reloads the Quantum Networking book from the database
        #F This shows that the PublishedOn date is what we expect
        * *******************************************************/

        [Fact]
        public void UpdatePublicationDateWithLoggingSqlServer()
        {
            //SETUP
            var showLog = false;
            //var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            var options = this.CreateUniqueClassOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using var context = new EfCoreContext(options);
            context.Database.EnsureClean();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            showLog = true;
            var book = context.Books                          
                .Single(p => p.Title == "Quantum Networking");
            book.PublishedOn = new DateTime(2058, 1, 1);         
            context.SaveChanges();                            
            showLog = false;

            //VERIFY
            var bookAgain = context.Books                     
                .Single(p => p.Title == "Quantum Networking");
            bookAgain.PublishedOn                             
                .ShouldEqual(new DateTime(2058, 1, 1));
        }

        [Fact]
        public void UpdateTwoPropertiesWithLogging()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            showLog = true;
            var book = context.Books
                .Single(p => p.Title == "Quantum Networking");
            book.Title = "New title";
            book.PublishedOn = new DateTime(2058, 1, 1);
            context.SaveChanges();
            showLog = false;

            //VERIFY
            var bookAgain = context.Books
                .Single(p => p.Title == "New title");
            bookAgain.PublishedOn
                .ShouldEqual(new DateTime(2058, 1, 1));
        }

        [Fact]
        public void UpdatePublicationDateDisconnected()
        {
            //SETUP
            ChangePubDateDto dto;
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            var service = new ChangePubDateService(context);
            dto = service.GetOriginal(4);
            dto.PublishedOn = new DateTime(2058, 1, 1);

            context.ChangeTracker.Clear();

            //ATTEMPT
            service.UpdateBook(dto);

            //VERIFY
            var bookAgain = context.Books.Single(p => p.BookId == dto.BookId);
            bookAgain.PublishedOn.ShouldEqual(new DateTime(2058, 1, 1));
        }

        [Fact]
        public void UpdateAuthorWithLogging()
        {
            //SETUP
            string json;
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            context.ChangeTracker.Clear();

            var author1 = context.Books                        //#A
                .Where(p => p.Title == "Quantum Networking")  //#A
                .Select(p => p.AuthorsLink.First().Author)    //#A
                .Single();                                    //#A
            author1.Name = "Future Person 2";                  //#A
            json = JsonConvert.SerializeObject(author1); //#A

            context.ChangeTracker.Clear();

            showLog = true;
            //ATTEMPT
            var author2 = JsonConvert
                .DeserializeObject<Author>(json);  //#B

            context.Update(author2); //#C                               
            context.SaveChanges();  //#D  
            /**********************************************************
            #A This simulates an external system returning a modified Author entity class as a JSON string
            #B This simulates receiving a JSON string from an external system and decoding it into an Author class
            #C I use the Update command, which replaces all the row data for the given primary key, in this case AuthorId
            * *******************************************************/

            //VERIFY
            var authorAgain = context.Books.Where(p => p.Title == "Quantum Networking")
                .Select(p => p.AuthorsLink.First().Author)
                .Single();
            authorAgain.Name.ShouldEqual("Future Person 2");
            context.Authors.Any(p =>
                p.Name == "Future Person").ShouldBeFalse();
        }

        [Fact]
        public void UpdateAuthorBadId()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var author = new Author
            {
                AuthorId = 999999,
                Name = "Future Person 2"
            };
            context.Authors.Update(author);
            var ex = Assert.Throws<DbUpdateConcurrencyException>(() => context.SaveChanges());

            //VERIFY
            ex.Message.StartsWith("Database operation expected to affect 1 row(s) but actually affected 0 row(s).").ShouldBeTrue();
        }

        [Fact]
        public void UpdateUpdatePublishedOnFromFourBooks()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            var books = context.Books.ToList();
            books.First().PublishedOn = new DateTime(2020, 1, 1);   
            context.SaveChanges();  

            //VERIFY
            context.Books.First().PublishedOn  
                .ShouldEqual(new DateTime(2020, 1, 1));
        }
    }
}