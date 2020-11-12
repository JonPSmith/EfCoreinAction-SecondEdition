// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.UI.HelperExtensions;
using BookApp.UI.Models;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TestSupport.Helpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestBookAppSettings
    {
        [Fact]
        public void TestBooAppSettings()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();

            //ATTEMPT
            var result = new BookAppSettings();
            config.GetSection(nameof(BookAppSettings)).Bind(result);

            //VERIFY
            result.CosmosAvailable.ShouldEqual(true);
            result.MenuSet.ShouldEqual(BookAppMenuSettings.Chapter15);
            result.DbNameSuffix.ShouldEqual("-Test");
            result.ProductionDbs.ShouldBeFalse();
        }

        [Theory]
        [InlineData(false, "Default-Test")]
        [InlineData(true, "Production-Test")]
        public void TestGetCorrectSqlConnectionProvidedSettings(bool productionDbs, string expectedDbName)
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = new BookAppSettings
            {
                DbNameSuffix = "-Test",
                ProductionDbs = productionDbs
            };

            //ATTEMPT
            var connection = config.GetCorrectSqlConnection(settings);

            //VERIFY
            var builder = new SqlConnectionStringBuilder(connection);
            builder["Initial Catalog"].ShouldEqual(expectedDbName);
        }

        [Fact]
        public void TestGetCorrectSqlConnectionReadOwnSettings()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();

            //ATTEMPT
            var connection = config.GetCorrectSqlConnection();

            //VERIFY
            var builder = new SqlConnectionStringBuilder(connection);
            builder["Initial Catalog"].ShouldEqual("Default-Test");
        }

        [Theory]
        [InlineData((string)null)]
        [InlineData("-Test")]
        public void TestGetCosmosDbSettingsProvidedSettings(string suffix)
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = new BookAppSettings
            {
                DbNameSuffix = suffix
            };

            //ATTEMPT
            var cosmosSettings = config.GetCosmosDbSettings(settings);

            //VERIFY
            cosmosSettings.EndPoint.ShouldNotBeEmpty();
            cosmosSettings.AuthKey.ShouldNotBeEmpty();
            cosmosSettings.DatabaseName.ShouldEqual("Cosmos" + (suffix ?? ""));
        }

        [Fact]
        public void TestGetCosmosDbSettingsCosmosAvailableFalse()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = new BookAppSettings
            {
                CosmosAvailable = false
            };

            //ATTEMPT
            var cosmosSettings = config.GetCosmosDbSettings(settings);

            //VERIFY
            cosmosSettings.ShouldBeNull();
        }
    }
}