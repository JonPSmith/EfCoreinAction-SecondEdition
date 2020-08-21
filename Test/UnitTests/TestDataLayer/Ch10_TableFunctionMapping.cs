// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter10Listings.EfCode;
using Test.TestHelpers;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch10_TableFunctionMapping
    {
        public Ch10_TableFunctionMapping(ITestOutputHelper output)
        {
            _output = output;

            _options = this.CreateUniqueClassOptions<Chapter10EfCoreContext>();
            using (var context = new Chapter10EfCoreContext(_options))
            {
                if (context.Database.EnsureCreated())
                {
                   //new database, so seed it with function and books
                    context.AddUdfToDatabase();

                    context.AddRange(EfTestData.CreateDummyBooks(setBookId: false));
                    context.SaveChanges();
                }
            }
        }

        private readonly ITestOutputHelper _output;

        private readonly DbContextOptions<Chapter10EfCoreContext> _options;


        [Fact]
        public void TestTableUdfOk()
        {
            //SETUP
            using (var context = new Chapter10EfCoreContext(_options))
            {
                //ATTEMPT
                var query = context.GetBookTitleAndReviewsFiltered(4);
                var results = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                results.Count.ShouldEqual(6);
            }
        }

        [Fact]
        public void TestTableUdfNullAverageVotesOk()
        {
            //SETUP
            using (var context = new Chapter10EfCoreContext(_options))
            {
                //ATTEMPT
                var query = context.GetBookTitleAndReviewsFiltered(0);
                var results = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                results.Count.ShouldEqual(10);
                results.Count(x => x.AverageVotes == null).ShouldEqual(1);
            }
        }

        [Fact]
        public void TestTableUdfWithExtraLinqOk()
        {
            //SETUP
            using (var context = new Chapter10EfCoreContext(_options))
            {
                //ATTEMPT
                var query = context.GetBookTitleAndReviewsFiltered(4)
                    .Where(x => x.AverageVotes > 2.5);
                var results = query.ToList();

                //VERIFY
                _output.WriteLine(query.ToQueryString());
                results.Count.ShouldEqual(5);
                _output.WriteLine(query.ToQueryString());
            }
        }


    }
}