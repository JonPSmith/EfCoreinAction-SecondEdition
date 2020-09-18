// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
    public class TestManningJsonToBooks
    {
        private ITestOutputHelper _output;

        public TestManningJsonToBooks(ITestOutputHelper output)
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
            var books = fileDir.LoadBooks("ManningBooks*.json", "ManningDetails*.json").ToList();

            //VERIFY
            books.Count.ShouldBeInRange(700, 800);
        }

        [Fact]
        public void TestLoadBooksCheckTagsOk()
        {
            //SETUP
            var callingAssemblyPath = TestData.GetCallingAssemblyTopLevelDir();
            var fileDir = Path.GetFullPath(Path.Combine(callingAssemblyPath, "..\\BookApp.UI\\wwwroot\\seedData"));

            //ATTEMPT
            var books10 = fileDir.LoadBooks("ManningBooks*.json", "ManningDetails*.json").Take(10).ToList();

            //VERIFY
            foreach (var book in books10)
            {
                _output.WriteLine(string.Join(", ", book.Tags.Select(x => x.TagId)));
            }
            books10.Any(x => x.Tags.Count > 0).ShouldBeTrue();
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
            var books10 = fileDir.LoadBooks("ManningBooks*.json", "ManningDetails*.json").Take(10).ToList();

            //ATTEMPT
            context.AddRange(books10);
            context.SaveChanges();

            //VERIFY
            context.Books.Count().ShouldEqual(10);
        }


    }
}