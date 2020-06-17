// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.SeedExample;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_SeedWithMirgation
    {

        [Fact]
        public void TestSeedingOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<SeedExampleDbContext>();
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
    }
}