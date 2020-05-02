// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter06Listings;
using TestSupport.Attributes;
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
                    foreach (var entityEntry in context.ChangeTracker.Entries())
                    {
                        if (entityEntry.Entity != null)
                        {
                            entityEntry.State = EntityState.Detached;
                        }
                    }
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
        [InlineData(10000)] //Comment out because it takes 25 seconds
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