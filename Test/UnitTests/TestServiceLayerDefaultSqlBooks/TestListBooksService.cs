// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Concrete;
using BookApp.ServiceLayer.DefaultSql.Books.QueryObjects;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestServiceLayerDefaultSqlBooks
{
    public class TestListBooksService
    {
        [Fact]
        public void TestMapBookToDto()
        {
            //SETUP
            var numBooks = 5;
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);

                //ATTEMPT
                var dtos = context.Books.MapBookToDto().ToList();

                //VERIFY
                dtos.Count.ShouldEqual(numBooks);
            }
        }

        [Theory]
        [InlineData(OrderByOptions.SimpleOrder)]
        [InlineData(OrderByOptions.ByPublicationDate)]
        public void OrderBooksBy(OrderByOptions orderByOptions)
        {
            //SETUP
            var numBooks = 5;
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);

                //ATTEMPT
                var service = new ListBooksService(context);
                var listOptions = new SortFilterPageOptions() { OrderByOptions = orderByOptions };
                var dtos = service.SortFilterPage(listOptions).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(numBooks);
            }
        }

        [Theory]
        [InlineData(5)]
        [InlineData(10)]
        public void PageBooks(int pageSize)
        {
            //SETUP
            var numBooks = 12;
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using (var context = new BookDbContext(options))
            {
                context.Database.EnsureCreated();
                context.SeedDatabaseDummyBooks(numBooks);

                //ATTEMPT
                var service = new ListBooksService(context);
                var listOptions = new SortFilterPageOptions() { PageSize = pageSize };
                var dtos = service.SortFilterPage(listOptions).ToList();

                //VERIFY
                dtos.Count.ShouldEqual(pageSize);
            }
        }
    }
}