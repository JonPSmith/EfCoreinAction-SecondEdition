// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter11Listings.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class TestToSqlQuery
    {
        private readonly ITestOutputHelper _output;

        public TestToSqlQuery(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookSqlQueriesToListOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SqlQueryDbContext>();
            using (var context = new SqlQueryDbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(EfTestData.CreateFourBooks());
                context.SaveChanges();

                //ATTEMPT
                var query = context.BookSqlQueries;
                var results = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                results.Select(x => x.AverageVotes).ToArray().ShouldEqual(new double?[] {null, null, null, 5});
            }
        }

        [Fact]
        public void TestBookSqlQueriesWithExtraLinqOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SqlQueryDbContext>();
            using (var context = new SqlQueryDbContext(options))
            {
                context.Database.EnsureCreated();
                context.AddRange(EfTestData.CreateFourBooks());
                context.SaveChanges();

                //ATTEMPT
                var query = context.BookSqlQueries.Where(x => x.AverageVotes > 3);
                var results = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                results.Count.ShouldEqual(1);
            }
        }
    }
}