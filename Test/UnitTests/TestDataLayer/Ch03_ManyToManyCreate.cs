// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch03_ManyToManyCreate
    {
        [Fact]
        public void TestCreateBookWithExistingAuthorsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();

            context.AddRange(new Author {Name = "Author1"}, new Author { Name = "Author2" });
            context.SaveChanges();
            
            context.ChangeTracker.Clear();
            
            //ATTEMPT
            var existingAuthor1 = context.Authors
                .Single(a => a.Name == "Author1");
            var existingAuthor2 = context.Authors
                .Single(a => a.Name == "Author2");
            var newBook = new Book()
            {
                Title = "My Book",
                //... other property settings left out

                //Set your AuthorsLink property to an empty collection
                AuthorsLink = new List<BookAuthor>()
            };
            newBook.AuthorsLink.Add(new BookAuthor
            {
                Book = newBook,
                Author = existingAuthor1,
                Order = 0
            });
            newBook.AuthorsLink.Add(new BookAuthor
            {
                Book = newBook,
                Author = existingAuthor2,
                Order = 1
            });
            context.Add(newBook);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var checkBook = context.Books
                .Include(book => book.AuthorsLink)
                .ThenInclude(bookAuthor => bookAuthor.Author)
                .Single();
            checkBook.AuthorsLink.Select(x => x.Author.Name).ShouldEqual(new[] { "Author1", "Author2" });
        }


        [Fact]
        public void TestCreateBookWithExistingTagsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<EfCoreContext>();
            using var context = new EfCoreContext(options);
            context.Database.EnsureCreated();

            context.AddRange(new Tag { TagId = "Tag1" }, new Tag { TagId = "Tag2" });
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var existingTag1 = context.Tags
                .Single(t => t.TagId == "Tag1");
            var existingTag2 = context.Tags
                .Single(t => t.TagId == "Tag2");
            var newBook = new Book()
            {
                Title = "My Book",
                //... other property settings left out
                
                //Set your Tags property to an empty collection 
                Tags = new List<Tag>()
            };
            newBook.Tags.Add(existingTag1);
            newBook.Tags.Add(existingTag2);
            context.Add(newBook);
            context.SaveChanges();

            context.ChangeTracker.Clear();

            //VERIFY
            var checkBook = context.Books
                .Include(book => book.Tags)
                .Single();
            checkBook.Tags.Select(x => x.TagId).ShouldEqual(new []{"Tag1", "Tag2"});
        }

    }
}