// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Domain.Books;
using BookApp.Persistence.EfCoreSql.Books;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestPersistenceNormalSqlBooks
{
    public class TestBookDbContext
    {
        [Fact]
        public void TestBookDbContextOk()
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
    }
}