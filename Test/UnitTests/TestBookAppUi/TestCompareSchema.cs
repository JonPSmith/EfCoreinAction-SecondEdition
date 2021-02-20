// Copyright (c) 2021 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.IO;
using System.Linq;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using EfSchemaCompare;
using Microsoft.EntityFrameworkCore;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestCompareSchema
    {
        [Fact]
        public void TestWriteBooksAsyncNoDataCausesNewDbOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var options1 = new DbContextOptionsBuilder<BookDbContext>().UseSqlServer(connection).Options;
            using var context1 = new BookDbContext(options1);
            var options2 = new DbContextOptionsBuilder<OrderDbContext>().UseSqlServer(connection).Options;
            using var context2 = new OrderDbContext(options2);

            context1.Database.EnsureDeleted();

            context1.Database.Migrate();
            context2.Database.Migrate();

            //ATTEMPT
            var comparer = new CompareEfSql();

            //ATTEMPT
            //Its starts with the connection string/name  and then you can have as many contexts as you like
            var hasErrors = comparer.CompareEfWithDb(context1, context2);

            //VERIFY
            hasErrors.ShouldBeFalse(comparer.GetAllErrors);
        }
    }
}