// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using Test.Chapter14Listings;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;

namespace Test.UnitTests.Chapter14
{
    public class TestNotMakingDetectChangesWorkHard
    {
        private ITestOutputHelper _output;

        public TestNotMakingDetectChangesWorkHard(ITestOutputHelper output)
        {
            _output = output;
        }


        [Theory]
        [InlineData(0)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void TestTimeTakenWithDifferentTracked(int numTracked)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter14DbContext>();
            using var context = new Chapter14DbContext(options);
            context.Database.EnsureCreated();

            AddMyEntities(context, numTracked);

            //ATTEMPT
            using (new TimeThings(_output, $"{numTracked} tracked"))
            {
                context.Add(new MyEntity());
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"{numTracked} tracked"))
            {
                context.Add(new MyEntity());
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"{numTracked} tracked"))
            {
                context.Add(new MyEntity());
                context.SaveChanges();
            }
            using (new TimeThings(_output, $"{numTracked} tracked"))
            {
                context.Add(new MyEntity());
                context.SaveChanges();
            }

            context.ChangeTracker.DetectChanges();

            //VERIFY

        }


        private void AddMyEntities(Chapter14DbContext context, int numToAdd)
        {
            var list = new List<MyEntity>();
            for (int i = 0; i < numToAdd; i++)
            {
                list.Add(new MyEntity());
            }
            context.AddRange(list);
            context.SaveChanges();
        }
    }
}