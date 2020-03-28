// Copyright (c) 2016 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.SeedDatabase;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_CoreUpdate
    {
        private readonly ITestOutputHelper _output;

        public Ch03_CoreUpdate(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void UpdatePublicationDate()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                var book = context.Books                          //#A
                    .Single(p => p.Title == "Quantum Networking");//#A
                book.PublishedOn = new DateTime(2058, 1, 1);      //#B     
                context.SaveChanges();                            //#C
                /**********************************************************
                #A This finds the specific book we want to update. In the case our special book on Quantum Networking
                #B Then it changes the expected publication date to year 2058 (it was 2057)
                #C It calls SaveChanges which includes running a method called DetectChanges. This spots that the PublishedOn property has been changed
                * *******************************************************/

                //VERIFY
                var bookAgain = context.Books                     //#E
                    .Single(p => p.Title == "Quantum Networking");//#E
                bookAgain.PublishedOn                             //#F
                    .ShouldEqual(new DateTime(2058, 1, 1));       //#F
            }
        }

        [Fact]
        public void UpdatePublicationDateWithLogging()
        {
            //SETUP
            var showLog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<EfCoreContext>(log =>
            {
                if (showLog)
                    _output.WriteLine(log.DecodeMessage());
            });
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                //ATTEMPT
                showLog = true;
                var book = context.Books                          //#B
                    .Single(p => p.Title == "Quantum Networking");//#B
                book.PublishedOn = new DateTime(2058, 1, 1);      //#C     
                context.SaveChanges();                            //#D

                //VERIFY
                var bookAgain = context.Books                     //#E
                    .Single(p => p.Title == "Quantum Networking");//#E
                bookAgain.PublishedOn                             //#F
                    .ShouldEqual(new DateTime(2058, 1, 1));       //#F

            }
        }
        /**********************************************************
        #A This create a database and seeds it with four books, one of which is the book on Quantum Networking
        #B It finds the specific book we want to update. In the case our special book on Quantum Networking
        #C Then it changes the expected publication date to year 2058 (it was 2057)
        #D It calls SaveChanges which includes running a method called DetectChanges. This spots that the PublishedOn property has been changed
        #E This reloads the Quantum Networking book from the database
        #F This shows that the PublishedOn date is what we expect
        * *******************************************************/


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
            using (var context = new EfCoreContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseFourBooks();

                var author = context.Books                                          //#A
                    .Where(p => p.Title == "Quantum Networking")                    //#A
                    .Select(p => p.AuthorsLink.First().Author)                      //#A
                    .Single();                                                      //#A
                author.Name = "Future Person 2";                                    //#A
                json = JsonConvert.SerializeObject(author,                          //#A
                    new JsonSerializerSettings()                                    //#A
                {
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects //#A
                });                                                                 //#A
            }                                                   

            using (var context = new EfCoreContext(options))
            {
                showLog = true;
                //ATTEMPT
                var author = JsonConvert
                    .DeserializeObject<Author>(json);  //#B

                context.Update(author); //#C                               
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
        }

        [Fact]
        public void UpdateAuthorBadId()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using (var context = new EfCoreContext(options))
            {
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
        }
    }
}