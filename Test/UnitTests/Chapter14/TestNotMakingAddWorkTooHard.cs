// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Test.Chapter14Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestNotMakingAddWorkTooHard
    {
        private readonly ITestOutputHelper _output;

        public TestNotMakingAddWorkTooHard(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestAddEntityWithCollections()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureCreated();

            AddCollection(context, 1, false);
            AddCollection(context, 100, false);
            AddCollection(context, 1000, false);
            context.ChangeTracker.Clear();

            //ATTEMPT
            AddCollection(context, 1000);
            AddCollection(context, 4000, false);
            AddCollection(context, 1000);
            AddCollection(context, 4000, false);
            AddCollection(context, 1000);
            AddCollection(context, 9000, false);
            AddCollection(context, 1000);
            AddCollection(context, 1000);

            //VERIFY
        }

        [Fact]
        public void TestAddEntityWithHashSets()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureCreated();

            AddHashSet(context, 1, false);
            AddHashSet(context, 100, false);
            AddHashSet(context, 1000, false);
            context.ChangeTracker.Clear();

            //ATTEMPT
            AddHashSet(context, 1000);
            AddHashSet(context, 4000, false);
            AddHashSet(context, 1000);
            AddHashSet(context, 4000, false);
            AddHashSet(context, 1000);
            AddHashSet(context, 9000, false);
            AddHashSet(context, 1000);
            AddHashSet(context, 1000);

            //VERIFY
        }

        //--------------------------------------------

        private void AddCollection(Chapter14DbContext context, int numRelations, bool showResults = true)
        {
            var count = context.ChangeTracker.Entries<SubEntity1>().Count();
            var entity = new MyEntity();
            for (int i = 0; i < numRelations; i++)
            {
                entity.Collections.Add(new SubEntity1());
            }
            
            using (new TimeThings(result =>
            {
                if (showResults)
                    _output.WriteLine(result.ToString());
            }, $"Add: Collection. #tracked = {count}", numRelations))
            {
                context.Add(entity);
            }
        }

        private void AddHashSet(Chapter14DbContext context, int numRelations, bool showResults = true)
        {
            var count = context.ChangeTracker.Entries<SubEntity2>().Count();
            var entity = new MyEntity();
            for (int i = 0; i < numRelations; i++)
            {
                entity.HashSets.Add(new SubEntity2());
            }
            using (new TimeThings(result =>
            {
                if (showResults)
                    _output.WriteLine(result.ToString());
            }, $"Add: HasSet. #tracked = {count}", numRelations))
            {
                context.Add(entity);
            }
        }
    }
}