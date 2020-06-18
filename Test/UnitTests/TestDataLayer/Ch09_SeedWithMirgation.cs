// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.SeedExample;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_SeedWithMirgation
    {
        private ITestOutputHelper _output;

        public Ch09_SeedWithMirgation(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestSeedingOnMigrateOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptionsWithLogging<SeedExampleDbContext>(log => _output.WriteLine(log.ToString()));
            using (var context = new SeedExampleDbContext(options))
            {
                //ATTEMPT
                context.Database.Migrate();

                //VERIFY
                var projects = context.Projects.Include(x => x.ProjectManager).ToList();
                projects.Count.ShouldEqual(2);
                projects.Select(x => x.ProjectManager).All(x => x != null).ShouldBeTrue();
            }
        }

        [Fact]
        public void TestSeedingOnEnsureCreatedOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SeedExampleDbContext>();
            using (var context = new SeedExampleDbContext(options))
            {
                //ATTEMPT
                context.Database.EnsureCreated();

                //VERIFY
                var projects = context.Projects.Include(x => x.ProjectManager).ToList();
                projects.Count.ShouldEqual(2);
                projects.Select(x => x.ProjectManager).All(x => x != null).ShouldBeTrue();
            }
        }
    }
}