// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using BookApp.Persistence.EfCoreSql.Books;
using Microsoft.EntityFrameworkCore;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter17Tests
{
    public class TestToQueryString
    {
        private readonly ITestOutputHelper _output;

        public TestToQueryString(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestToQueryStringOnLinqQuery()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var query = context.Books.Select(x => x.BookId); //#A
            var bookIds = query.ToArray(); //#B

            //VERIFY
            _output.WriteLine(query.ToQueryString()); //#C
            query.ToQueryString().ShouldEqual(         //#D
                "SELECT \"b\".\"BookId\"\r\n" +        //#D
                "FROM \"Books\" AS \"b\"\r\n" +        //#D
                "WHERE NOT (\"b\".\"SoftDeleted\")");  //#D
            bookIds.ShouldEqual(new []{1,2,3,4});      //#E
        }
        /****************************************************************
        #A You provide the LINQ query without an execution part
        #B Then you run the LINQ query by adding ToArray on the end
        #C This will output the SQL for your LINQ query
        #D Here I test than the SQL is what I expected
        #E Finally I test the output of the query
         ****************************************************************/

        [Fact]
        public void TestToQueryStringOnSqlQuery()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<BookDbContext>();
            using var context = new BookDbContext(options);
            context.Database.EnsureCreated();
            context.SeedDatabaseFourBooks();

            //ATTEMPT 
            var query = context.Books.FromSqlInterpolated($"SELECT b.BookId FROM Books");

            //VERIFY
            query.ToQueryString().ShouldEqual("SELECT \"b\".\"BookId\", \"b\".\"ActualPrice\", \"b\".\"AuthorsOrdered\"," +
                                              " \"b\".\"EstimatedDate\", \"b\".\"ImageUrl\", \"b\".\"LastUpdatedUtc\"," +
                                              " \"b\".\"ManningBookUrl\", \"b\".\"OrgPrice\", \"b\".\"PromotionalText\"," +
                                              " \"b\".\"PublishedOn\", \"b\".\"Publisher\", \"b\".\"ReviewsAverageVotes\"," +
                                              " \"b\".\"ReviewsCount\", \"b\".\"SoftDeleted\", \"b\".\"Title\"," +
                                              " \"b\".\"WhenCreatedUtc\"\r\n" +
                                              "FROM (\r\n    SELECT b.BookId FROM Books\r\n) AS \"b\"\r\nWHERE NOT (\"b\".\"SoftDeleted\")");
        }
    }
}