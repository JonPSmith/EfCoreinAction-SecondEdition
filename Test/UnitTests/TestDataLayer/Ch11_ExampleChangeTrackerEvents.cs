// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Test.Chapter11Listings.EfClasses;
using Test.Chapter11Listings.EfCode;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch11_ExampleChangeTrackerEvents
    {
        private readonly ITestOutputHelper _output;

        public Ch11_ExampleChangeTrackerEvents(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestTrackedEventOnQueryOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<Chapter11DbContext>();
            var logs = new List<LogOutput>();
            var logProvider = new MyLoggerProvider(logs);
            using (var context = new Chapter11DbContext(options, logProvider.CreateLogger("test")))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var entity = new MyEntity {MyString = "Test"};
                context.Add(entity);
                context.SaveChanges();
                entity.MyString = "new Name";
                context.SaveChanges();
            }
            using (var context = new Chapter11DbContext(options, logProvider.CreateLogger("test")))
            {
                var readIn = context.MyEntities.Single();

                //VERIFY
                logs.Count.ShouldEqual(4);
                foreach (var logOutput in logs)
                {
                    _output.WriteLine(logOutput.Message);
                }
            }
        }
    }
}