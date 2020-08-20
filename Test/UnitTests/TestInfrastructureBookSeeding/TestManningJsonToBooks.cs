// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BookApp.Domain.Books;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestInfrastructureBookSeeding
{
    public class TestManningJsonToBooks
    {
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