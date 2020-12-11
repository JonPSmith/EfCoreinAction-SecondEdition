// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestUsingTransactionInUnitTests
    {
        private readonly ITestOutputHelper _output;
        private readonly string _connectionString;

        public TestUsingTransactionInUnitTests(ITestOutputHelper output)
        {
            _output = output;
            _connectionString = this.GetUniqueDatabaseConnectionString();
            var builder = new
                DbContextOptionsBuilder<BookDbContext>();
            builder.UseSqlServer(_connectionString);
            using var context = new BookDbContext(builder.Options);
            if (context.Database.EnsureCreated())
                context.SeedDatabaseFourBooks();
        }

        [Fact]
        public void TestUsingTransactionToRollBackChanges()
        {
            //SETUP
            var builder = new                            //#A
                DbContextOptionsBuilder<BookDbContext>();//#A
            builder.UseSqlServer(_connectionString);     //#A
            using var context =                          //#A
                new BookDbContext(builder.Options);      //#A


            using var transaction =                  //#B
                context.Database.BeginTransaction(); //#B

            //ATTEMPT 
            var newBooks = BookTestData //#C
                .CreateDummyBooks(10);  //#C
            context.AddRange(newBooks); //#C
            context.SaveChanges();      //#C

            //VERIFY
            context.Books.Count().ShouldEqual(4+10); //#E

        } //#F

        /****************************************************************
        #A You most likely will link to a database via a connection string
        #B The transaction is held in a user var variable, which means it will be disposed when the current block ends
        #C Then you run your test
        #D And check it worked
        #E When the unit test method ends the transaction will be disposed and will roll back the changes made in the unit test 
         ****************************************************************/


    }
}