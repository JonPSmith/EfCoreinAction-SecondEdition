// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestSupportCode
{
    public class Ch02_AppSettings
    {
        [Fact]
        public void GetConfigurationOk()
        {
            //SETUP

            //ATTEMPT
            var config = AppSettings.GetConfiguration();

            //VERIFY
            config.GetConnectionString("UnitTestConnection")
                .ShouldEqual("Server=(localdb)\\mssqllocaldb;Database=EfCoreInActionDb2-Test;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        [Fact]
        public void GetTestConnectionStringOk()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var orgDbName = new SqlConnectionStringBuilder(config.GetConnectionString("UnitTestConnection"))
                .InitialCatalog;

            //ATTEMPT
            var con = this.GetUniqueDatabaseConnectionString();

            //VERIFY
            var newDatabaseName = new SqlConnectionStringBuilder(con).InitialCatalog;
            Assert.StartsWith($"{orgDbName}_", newDatabaseName);
            Assert.EndsWith($"{typeof(Ch02_AppSettings).Name}", newDatabaseName);
        }


        [Fact]
        public void GetTestConnectionStringWithExtraMethodNameOk()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var orgDbName = new SqlConnectionStringBuilder(config.GetConnectionString("UnitTestConnection"))
                .InitialCatalog;

            //ATTEMPT
            var con = this.GetUniqueDatabaseConnectionString("ExtraMethodName");

            //VERIFY
            var newDatabaseName = new SqlConnectionStringBuilder(con).InitialCatalog;
            Assert.StartsWith($"{orgDbName}_", newDatabaseName);
            Assert.EndsWith($"{typeof(Ch02_AppSettings).Name}_ExtraMethodName", newDatabaseName);
        }
    }
}