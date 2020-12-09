// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch06_AnNoTrackingAfterRead
    {
        private readonly ITestOutputHelper _output;

        public Ch06_AnNoTrackingAfterRead(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ExampleDetachViaCreatingNewDbContext()
        {
            //SETUP

            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            options.StopNextDispose();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                context.AddManyTopWithRelationsToDb();
            }

            //ATTEMPT
            ManyTop entityToDetach;                                 //#A
            using (var tempContext = new Chapter06Context(options)) //#B
            {
                entityToDetach = tempContext.ManyTops.Single();     //#C
                var a = tempContext.Set<Many1>()                    //#D
                    .Where(x => x.ManyTopId == entityToDetach.Id)   //#D
                    .ToList();                                      //#D
                //... rest of loads left out to shorten the example
            }                                                       //#E
            //… further code that uses the entityToDetach variable  //#F
            /************************************************************************
            #A This is the variable which will hold the detached entity class with its relationships that were read in separately
            #B Create a new instance of the application's DbContext. This must be done manually because you should not dispose of an instance that has been created by dependency injections
            #C Read of the top level entity class
            #D Read of the Many1 instances. Relational fixup will fill in the navigational collection in the ManyTop class which holds the loaded Many1 instances
            #E The closing of the using block means that the tempContext is disposed, including all ist tracking data
            #F As this point all the entity class instances read in while in the using block are detached from any DbContext
            * ********************************/

            //VERIFY
            entityToDetach.Collection1.Count.ShouldEqual(100);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        public void TestCostOfNotDetaching(int numCollection)
        {
            //SETUP
            ManyTop entityToDetach;
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                using (new TimeThings(_output, "SaveChanges - no tracked entities."))
                {
                    context.SaveChanges();
                }
                using (new TimeThings(_output, "SaveChanges - no tracked entities."))
                {
                    context.SaveChanges();
                }

                entityToDetach = context.AddManyTopWithRelationsToDb(numCollection);
                var numEntities = entityToDetach.Collection1.Count + 
                                  entityToDetach.Collection2.Count +
                                  entityToDetach.Collection3.Count;


                using (new TimeThings(_output, $"SaveChanges - {numEntities:n0} tracked entities."))
                {
                    context.SaveChanges();
                }
                using (new TimeThings(_output, $"SaveChanges - {numEntities:n0} tracked entities."))
                {
                    context.SaveChanges();
                }
            }

            //VERIFY
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        public void TestDetachViaCreatingNewDbContext(int numCollection)
        {
            //SETUP
            ManyTop entityToDetach;
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            options.StopNextDispose();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                entityToDetach = context.AddManyTopWithRelationsToDb(numCollection);
            }
            //ATTEMPT
            using (new TimeThings(_output, $"detach by creating new DbContext - {numCollection * 3:n0} tracked entities."))
            {
                using (var context = new Chapter06Context(options))
                {
                    context.Database.EnsureCreated();
                }
            }

            //VERIFY
        }


        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        public void TestDetachAllTrackedEntities(int numCollection)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter06Context>();
            using (var context = new Chapter06Context(options))
            {
                context.Database.EnsureCreated();
                var entityToDetach = context.AddManyTopWithRelationsToDb();
                //ATTEMPT
                using (new TimeThings(_output, $"detach all tracked entities - {numCollection * 3:n0} tracked entities."))
                {
                    context.ChangeTracker.Clear();
                }

                //VERIFY
                var topEntity = context.Entry(entityToDetach);
                entityToDetach.Collection1.All(x => context.Entry(x).State == EntityState.Detached).ShouldBeTrue();
                entityToDetach.Collection2.All(x => context.Entry(x).State == EntityState.Detached).ShouldBeTrue();
                entityToDetach.Collection3.All(x => context.Entry(x).State == EntityState.Detached).ShouldBeTrue();
            }
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1000)]
        //[InlineData(10000)] //Comment out because it takes 25 seconds
        public void TestCombinedCalls(int numCollection)
        {
            TestDetachAllTrackedEntities(numCollection);
            TestDetachAllTrackedEntities(numCollection);
            TestDetachViaCreatingNewDbContext(numCollection);
            TestDetachViaCreatingNewDbContext(numCollection);
            TestDetachAllTrackedEntities(numCollection);
            TestDetachViaCreatingNewDbContext(numCollection);
            TestCostOfNotDetaching(numCollection);
        }


    }
}