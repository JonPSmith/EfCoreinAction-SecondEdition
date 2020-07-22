// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.SeedExample;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_SeedWithMigration
    {
        private readonly ITestOutputHelper _output;

        public Ch09_SeedWithMigration(ITestOutputHelper output)
        {
            _output = output;
        }

        [RunnableInDebugOnly]
        public void TestSeedingOnMigrateSqlServerOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<SeedExampleDbContext>();
            using (var context = new SeedExampleDbContext(options))
            {
                //ATTEMPT
                context.Database.EnsureDeleted();
                context.Database.Migrate();

                //VERIFY
                var projects = context.Projects.Include(x => x.ProjectManager).ToList();
                projects.Count.ShouldEqual(2);
                projects.Select(x => x.ProjectManager).All(x => x != null).ShouldBeTrue();
                
            }
        }

        [Fact]
        public void TestSeedingOnMigrateSqliteOk()
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
                projects.Select(x => x.ProjectManager.ToString()).ShouldEqual(new []
                {
                    "UserId: 1, Name: NEW Jill, Street: Street1, City: city1, ProjectId: 1",
                    "UserId: 3, Name: Jack3, Street: Street3, City: city2, ProjectId: 2"
                });
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
                projects.Select(x => x.ProjectManager.ToString()).ShouldEqual(new[]
                {
                    "UserId: 1, Name: NEW Jill, Street: Street1, City: city1, ProjectId: 1",
                    "UserId: 3, Name: Jack3, Street: Street3, City: city2, ProjectId: 2"
                });
            }
        }

        [Fact]
        public void TestSeedingOnEnsureCleanOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptions<SeedExampleDbContext>();
            using (var context = new SeedExampleDbContext(options))
            {
                //ATTEMPT
                context.Database.EnsureClean();

                //VERIFY
                var projects = context.Projects.Include(x => x.ProjectManager).ToList();
                projects.Count.ShouldEqual(2);
                projects.Select(x => x.ProjectManager.ToString()).ShouldEqual(new[]
                {
                    "UserId: 1, Name: NEW Jill, Street: Street1, City: city1, ProjectId: 1",
                    "UserId: 3, Name: Jack3, Street: Street3, City: city2, ProjectId: 2"
                });
            }
        }
    }
}