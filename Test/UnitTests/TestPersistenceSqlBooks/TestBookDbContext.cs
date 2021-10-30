// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceSqlBooks
{
    public class TestBookDbContext
    {
        private readonly ITestOutputHelper _output;

        public TestBookDbContext(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookDbContextAddFourBooksOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            context.SeedDatabaseFourBooks();

            //VERIFY
            context.Books.Count().ShouldEqual(4);
            context.Authors.Count().ShouldEqual(3);
            context.Set<Review>().Count().ShouldEqual(2);
        }

        [Fact]
        public void TestBookDbContextAddFourBooksLoadRelationshipsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT
            context.ChangeTracker.Clear();
            var books = context.Books.Include(x => x.Reviews)
                .Include(x => x.AuthorsLink).ThenInclude(x => x.Author).ToList();

            //VERIFY
            books.All(x => x.AuthorsLink.Single().Author != null).ShouldBeTrue();
            books.Last().Reviews.Count().ShouldEqual(2);
        }

        [Fact]
        public void TestBookDbContextBookWithTagsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var status = Book.CreateBook("title", new DateTime(2020,1,1), false, 
                "Manning", 123, "imageRul", 
                new List<Author>{new Author("author1", null)},
                new List<Tag>{new Tag("tag1"), new Tag("tag2")});
            context.Add(status.Result);
            context.SaveChanges();

            //VERIFY
            status.IsValid.ShouldBeTrue(status.GetAllErrors());
            context.Books.Count().ShouldEqual(1);
            context.Authors.Count().ShouldEqual(1);
            context.Tags.Count().ShouldEqual(2);
        }
    }
}