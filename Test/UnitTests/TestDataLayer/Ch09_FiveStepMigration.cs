// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;
using Test.Chapter09Listings.FiveStepMigration;

using TestSupport.EfHelpers;
using TestSupport.EfSchemeCompare;
using TestSupportSchema;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestDataLayer
{
    public class Ch09_FiveStepMigration
    {
        private readonly ITestOutputHelper _output;

        public Ch09_FiveStepMigration(ITestOutputHelper output)
        {
            _output = output;
        }


        [Fact]
        public void EmulateAFiveStepMigrationOfContinuousRunningAppOk()
        {
            //SETUP
            var app1Options = this.CreateUniqueClassOptions<App1DbContext>();
            var app2Options = this.CreateUniqueClassOptions<App2DbContext>();
            var app3Options = this.CreateUniqueClassOptions<App3DbContext>();
            //APP1 RUNNING
            using (var app1Context = new App1DbContext(app1Options))
            {
                app1Context.Database.EnsureClean();

                //APP1
                app1Context.Add(new UserPart1 {Name = "Added by App1, step1", Street = "App1 street", City = "App1 city"});
                app1Context.SaveChanges();

                //APPLY 1st migration
                Migration1(app1Context);

                //APP2 RUNNING
                using (var app2Context = new App2DbContext(app2Options))
                {
                    CheckDatabaseMatchesDbContext(app2Context);

                    app2Context.Add(new UserPart2
                    {
                        Name = "Added by App2, step 2",
                        Address = new Address {Street = "App2 street", City = "App2 city"}
                    });
                    app2Context.SaveChanges();
                    app1Context.Add(new UserPart1 { Name = "Added by App1, step2", Street = "App1 street", City = "App1 city" });
                    app1Context.SaveChanges();

                    var userWithAddressViaView = app2Context.ReadOnlyUserWithAddresses.ToList();
                    userWithAddressViaView.Count.ShouldEqual(3);
                    userWithAddressViaView.All(x => x.Street != null).ShouldBeTrue();
                }
            }
            //STAGE 3 - APP1 stopped and copy over 
            using (var app2Context = new App2DbContext(app2Options))
            {
                Migration2CopyUserAddressToAddresses(app2Context);

                var userWithAddressReal = app2Context.Users.Include(x => x.Address).ToList();
                userWithAddressReal.Count.ShouldEqual(3);
                userWithAddressReal.All(x => x.Address != null).ShouldBeTrue();

                var userWithAddressViaView = app2Context.ReadOnlyUserWithAddresses.ToList();
                userWithAddressViaView.Count.ShouldEqual(3);
                userWithAddressViaView.All(x => x.Street != null).ShouldBeTrue();
            }
        }

        private void CheckDatabaseMatchesDbContext(DbContext context)
        {
            var config = new CompareEfSqlConfig();
            config.IgnoreTheseErrors("WARNING: Database 'EfSchemaCompare does not check read-only types'. Expected = <null>, found = ReadOnlyUserWithAddress");
            var comparer = new CompareEfSql(config);
            var hasErrors = comparer.CompareEfWithDb(context);
            hasErrors.ShouldBeFalse(comparer.GetAllErrors);
        }

        private void Migration1(DbContext context)
        {
            context.Database.ExecuteSqlRaw(@"CREATE TABLE [Addresses] (
    [AddressId] int NOT NULL IDENTITY,
    [Street] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    CONSTRAINT [PK_Addresses] PRIMARY KEY ([AddressId])
);");
            context.Database.ExecuteSqlRaw("ALTER TABLE [Users] ADD [AddressId] int NULL");
            context.Database.ExecuteSqlRaw("ALTER TABLE [Users] ADD CONSTRAINT [FK_Users_Addresses_AddressId] " +
                                           "FOREIGN KEY ([AddressId]) REFERENCES [Addresses] ([AddressId]) ON DELETE NO ACTION");
            context.Database.ExecuteSqlRaw("CREATE INDEX [IX_Users_AddressId] ON [Users] ([AddressId]);");

            context.Database.ExecuteSqlRaw(@"CREATE VIEW [GetUserWithAddress] AS
SELECT UserId,
   Name,
   CASE
      WHEN users.AddressId IS NULL THEN Street
	  ELSE (Select Street FROM Addresses AS addr WHERE users.AddressId = addr.AddressId)
	END AS Street,
	CASE
      WHEN AddressId IS NULL THEN City
	  ELSE (Select City FROM Addresses AS addr WHERE users.AddressId = addr.AddressId)
	END AS City
FROM Users as users");
        }

        private void Migration2CopyUserAddressToAddresses(DbContext context)
        {
            //This code was taken from the earlier MoveColumns example - with alterations
            context.Database.ExecuteSqlRaw("ALTER TABLE [Addresses] ADD [UserId] [int] NULL");
            context.Database.ExecuteSqlRaw(@"INSERT INTO [Addresses] ([UserId],[Street],[City])
SELECT [UserId],[Street],[City] FROM [Users] AS users WHERE users.AddressId IS NULL");
            context.Database.ExecuteSqlRaw(@"UPDATE [Users] 
SET [AddressId] = (SELECT [AddressId] 
    FROM [Addresses] 
    WHERE [Addresses].[UserId] = [Users].[Userid])
	WHERE [AddressId] IS NULL");
            context.Database.ExecuteSqlRaw("ALTER TABLE [Addresses] DROP COLUMN [UserId]");
        }

        [Fact]
        public void GetApp1CreateTablesOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptionsWithLogging<App1DbContext>(log => _output.WriteLine(log.Message));
            using (var context = new App1DbContext(options))
            {
                context.Database.EnsureClean();
            }
        }

        [Fact]
        public void GetApp2CreateTablesOk()
        {
            //SETUP
            var options = this.CreateUniqueClassOptionsWithLogging<App2DbContext>(log => _output.WriteLine(log.Message));
            using (var context = new App2DbContext(options))
            {
                context.Database.EnsureClean();
            }
        }
    }
}