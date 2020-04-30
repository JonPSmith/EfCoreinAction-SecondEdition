// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
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
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                context.AddManyTopWithRelationsToDb();
            }
            using (var context = new Chapter06Context(options))
            {
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
        }

        [Fact]
        public void TestBookQueryWithSeparateIncludes()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                context.AddManyTopWithRelationsToDb();
            }
            using (var context = new Chapter06Context(options))
            {
                //ATTEMPT
                var dummy1 = context.ManyTops.ToList();
                var dummy2 = context.ManyTops.ToList();
                ManyTop result;
                using (new TimeThings(_output, "sync load - first time"))
                {
                    result = context.ManyTops.Single(); //#A
                    var a = context.Set<Many1>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToList(); //#B
                    var b = context.Set<Many2>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToList(); //#B
                    var c = context.Set<Many3>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToList(); //#B

                    /*********************************************************
                    #A This read in the main entity class, ManyTop that the relationships link to first
                    #B Then you read in the collections one by one. Relational fixup will fill in the navigational properties in the main entity class, ManyTop  
                     **********************************************************/
                }
                using (new TimeThings(_output, "sync load - second time"))
                {
                    result = context.ManyTops.Single();
                    var loadRels = new
                    {
                        a = context.Set<Many1>().Where(x => x.ManyTopId == result.Id).ToList(),
                        b = context.Set<Many2>().Where(x => x.ManyTopId == result.Id).ToList(),
                        c = context.Set<Many3>().Where(x => x.ManyTopId == result.Id).ToList(),
                    };
                }

                //VERIFY
                result.Collection1.Count.ShouldEqual(100);
                result.Collection2.Count.ShouldEqual(100);
                result.Collection3.Count.ShouldEqual(100);
            }
        }

        [Fact]
        public async Task TestBookQueryWithSeparateIncludesAsync()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                context.AddManyTopWithRelationsToDb();
            }
            using (var context = new Chapter06Context(options))
            {
                //ATTEMPT
                var dummy1 = context.ManyTops.ToList();
                var dummy2 = context.ManyTops.ToList();
                ManyTop result;
                using (new TimeThings(_output, "async load - first time"))
                {
                    result = context.ManyTops.Single(); //#A
                    var a = await context.Set<Many1>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToListAsync(); //#B
                    var b = await context.Set<Many2>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToListAsync(); //#B
                    var c = await context.Set<Many3>()                        //#B
                        .Where(x => x.ManyTopId == result.Id).ToListAsync(); //#B

                    /*********************************************************
                    #A This read in the main entity class, ManyTop that the relationships link to first
                    #B Then you read in the collections one by one. Relational fixup will fill in the navigational properties in the main entity class, ManyTop  
                     **********************************************************/
                }
                using (new TimeThings(_output, "async load - second time"))
                {
                    result = context.ManyTops.Single();
                    var loadRels = new
                    {
                        a = context.Set<Many1>().Where(x => x.ManyTopId == result.Id).ToList(),
                        b = context.Set<Many2>().Where(x => x.ManyTopId == result.Id).ToList(),
                        c = context.Set<Many3>().Where(x => x.ManyTopId == result.Id).ToList(),
                    };
                }

                //VERIFY
                result.Collection1.Count.ShouldEqual(100);
                result.Collection2.Count.ShouldEqual(100);
                result.Collection3.Count.ShouldEqual(100);
            }
        }




    }
}