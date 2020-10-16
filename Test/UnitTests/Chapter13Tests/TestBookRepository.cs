// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter13Listings.EfClasses;
using Test.Chapter13Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter13Tests
{
    public class TestBookRepository
    {
        [Fact]
        public void TestCreateBookOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DddContext>();
            using var context = new DddContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var status = Book.CreateBook("Test", new DateTime(2020, 1, 1),
                123, new List<Author> {new Author("Author1", "a@gmail.com")});
            status.IsValid.ShouldBeTrue(status.GetAllErrors());
            context.Add(status.Result);
            context.SaveChanges();

            //VERIFY
            context.ChangeTracker.Clear();
            var bookWithAuthor = context.Books
                .Include(x => x.AuthorsLink).ThenInclude(x => x.Author)
                .Single();
            bookWithAuthor.Title.ShouldEqual("Test");
            bookWithAuthor.AuthorsLink.SingleOrDefault()?.Author?.Name.ShouldEqual("Author1");

        }
    }
}