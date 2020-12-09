// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_FixingMultipleIncludes
    {
        private readonly ITestOutputHelper _output;

        public Ch06_FixingMultipleIncludes(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestBookQueryWithNormalIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();

            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddManyTopWithRelationsToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var dummy1 = context.ManyTops.ToList();
            var dummy2 = context.ManyTops.ToList();
            ManyTop result;
            using (new TimeThings(_output, "normal includes - first time"))
            {
                result = context.ManyTops
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .Single();
            }
            using (new TimeThings(_output, "normal includes - second time"))
            {
                result = context.ManyTops
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .Single();
            }

            //VERIFY
            result.Collection1.Count.ShouldEqual(100);
            result.Collection2.Count.ShouldEqual(100);
            result.Collection3.Count.ShouldEqual(100);
        }

        [Fact]
        public void TestBookQueryWithSeparateIncludes()
        {
            //SETUP
            var showlog = false;
            var options = SqliteInMemory.CreateOptionsWithLogging<Chapter06Context>(log =>
            {
                if (showlog)
                    _output.WriteLine(log.Message);
            });
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddManyTopWithRelationsToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var dummy1 = context.ManyTops.ToList();
            var dummy2 = context.ManyTops.ToList();
            ManyTop result;
            var id = 1;
            using (new TimeThings(_output, "sync load - first time"))
            {
                result = context.ManyTops
                    .AsSplitQuery() //#A
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .Single(x => x.Id == id);

                /*********************************************************
                    #A This will cause each Include to be loaded separately, thus stopping the multiplication problem 
                     **********************************************************/
            }
            using (new TimeThings(_output, "sync load - second time"))
            {
                result = context.ManyTops
                    .AsSplitQuery()
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .Single();
            }

            //VERIFY
            result.Collection1.Count.ShouldEqual(100);
            result.Collection2.Count.ShouldEqual(100);
            result.Collection3.Count.ShouldEqual(100);
        }

        [Fact]
        public async Task TestBookQueryWithSeparateIncludesAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using var context = new Chapter06Context(options);
            context.Database.EnsureCreated();
            context.AddManyTopWithRelationsToDb();

            context.ChangeTracker.Clear();

            //ATTEMPT
            var dummy1 = context.ManyTops.ToList();
            var dummy2 = context.ManyTops.ToList();
            ManyTop result;
            using (new TimeThings(_output, "async load - first time"))
            {
                result = await context.ManyTops
                    .AsSplitQuery()
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .SingleAsync();

                /*********************************************************
                    #A This read in the main entity class, ManyTop that the relationships link to first
                    #B Then you read in the collections one by one. Relational fixup will fill in the navigational properties in the main entity class, ManyTop  
                     **********************************************************/
            }
            using (new TimeThings(_output, "async load - second time"))
            {
                result = await context.ManyTops
                    .AsSplitQuery()
                    .Include(x => x.Collection1)
                    .Include(x => x.Collection2)
                    .Include(x => x.Collection3)
                    .SingleAsync();
            }

            //VERIFY
            result.Collection1.Count.ShouldEqual(100);
            result.Collection2.Count.ShouldEqual(100);
            result.Collection3.Count.ShouldEqual(100);
        }




    }
}