// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.TwoDbContexts;
using TestSupport.Helpers;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_MultipleDbsOnDatabase
    {
        private readonly ITestOutputHelper _output;

        public Ch09_MultipleDbsOnDatabase(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void TestMigrateEachPartOfDbOk()
        {
            //SETUP
            var connection = this.GetUniqueDatabaseConnectionString();
            var optionsBuilder1 = new DbContextOptionsBuilder<DbContext1>();
            optionsBuilder1.UseSqlServer(connection,
                x => x.MigrationsHistoryTable($"__{nameof(DbContext1)}"));
            using (var context = new DbContext1(optionsBuilder1.Options))
            {
                context.Database.EnsureClean(false);

                //ATTEMPT
                context.Database.Migrate();

                context.Add(new Shared {SharedString = "Created in Db1"});
                context.SaveChanges();
            }
            var optionsBuilder2 = new DbContextOptionsBuilder<DbContext2>();
            optionsBuilder2.UseSqlServer(connection,
                x => x.MigrationsHistoryTable($"__{nameof(DbContext2)}"));
            using (var context = new DbContext2(optionsBuilder2.Options))
            {
                //ATTEMPT
                context.Database.Migrate();

                //VERIFY
                var shared = context.Shared.SingleOrDefault();
                shared.ShouldNotBeNull();
                shared.SharedString.ShouldEqual("Created in Db1");
            }
        }
    }
}