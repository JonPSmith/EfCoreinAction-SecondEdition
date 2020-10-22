// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper.Configuration.Annotations;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestManningBookLoad
    {
        private readonly ITestOutputHelper _output;

        public TestManningBookLoad(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestLoadBooksOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));

            //ATTEMPT
            var loadedBooks = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //VERIFY
            loadedBooks.Books.Count().ShouldBeInRange(700, 800);
            loadedBooks.AuthorsDict.Values.Count.ShouldBeInRange(800,1000);
            loadedBooks.TagsDict.Values.Count.ShouldBeInRange(30, 40);
            loadedBooks.Books.Count(x => x.Details?.Description != null).ShouldBeInRange(700, 750);
        }

        [Fact]
        public void TestLoadBooksCheckTagsOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));

            //ATTEMPT
            var loadedBooks = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //VERIFY
            foreach (var book in loadedBooks.Books.Take(10))
            {
                _output.WriteLine(string.Join(", ", book.Tags.Select(x => x.TagId)));
                book.Tags.Any().ShouldBeTrue();
            }
        }

        [Fact]
        public void TestManuallyAddDetailsOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var book = BookTestData.CreateDummyBookOneAuthor();
            book.SetBookDetails("d","aa", "ar", "at", "w");
            context.Add(book);
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(1);
        }

        [Fact]
        public void TestLoadBooksAddTenToDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();

            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));

            var loadedBooks = new ManningBookLoad(fileDir, "ManningBooks*.json", "ManningDetails*.json");

            //ATTEMPT
            context.AddRange(loadedBooks.Books.Take(10));
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(10);
            context.Authors.Count().ShouldBeInRange(10, 20);
            context.Tags.Count().ShouldBeInRange(5,15);

        }


    }
}