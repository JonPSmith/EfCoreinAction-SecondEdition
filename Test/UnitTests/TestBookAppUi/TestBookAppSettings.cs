// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Infrastructure.AppParts;
using BookApp.UI.HelperExtensions;
using BookApp.UI.Models;
using BookApp.UI.Services;
using TestSupport.Helpers;
using Xunit;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestBookAppSettings
    {
        private readonly ITestOutputHelper _output;

        public TestBookAppSettings(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestGetBookAppSettingsUsingSetupVersionNum()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();

            //ATTEMPT
            var result = config.GetBookAppSettings();

            //VERIFY
            result.Title.ShouldEqual("Setup1");
            result.MenuSet.ShouldEqual(BookAppMenuSettings.Basic);
            result.SqlConnectionString.ShouldEqual("Setup1: sql connection string");
            result.CosmosAvailable.ShouldBeFalse();
        }

        [Fact]
        public void TestGetBookAppSettingsList()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();

            //ATTEMPT
            

            //VERIFY
            for (int versionNum = 1; versionNum <= 3; versionNum++)
            {
                var settings = config.GetBookAppSettings(versionNum);
                _output.WriteLine(settings.ToString());
            }
        }


        [Theory]
        [InlineData(1, "Setup1: sql connection string")]
        [InlineData(2, "Setup2: sql connection string")]
        public void TestGetBookAppSettings(int versionNum, string expectedConn)
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = config.GetBookAppSettings(versionNum);

            //ATTEMPT
            var connection = config.GetCorrectSqlConnection(settings);

            //VERIFY
            connection.ShouldEqual(expectedConn);
        }

        [Theory]
        [InlineData(1, null, null)]
        [InlineData(3, "Setup3: cosmos connection string", "BookAppCosmos3")]
        public void TestGetCosmosDbSettings(int versionNum, string expectedConn, string expectedDbName)
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = config.GetBookAppSettings(versionNum);

            //ATTEMPT
            var cosmosSettings = config.GetCosmosDbSettings(settings);

            //VERIFY
            cosmosSettings?.ConnectionString.ShouldEqual(expectedConn);
            cosmosSettings?.DatabaseName.ShouldEqual(expectedDbName);
        }

        [Fact]
        public void TestGetCosmosDbSettingsEmulator()
        {
            //SETUP
            var config = AppSettings.GetConfiguration();
            var settings = config.GetBookAppSettings(4);

            //ATTEMPT
            var cosmosSettings = config.GetCosmosDbSettings(settings);

            //VERIFY
            cosmosSettings.ConnectionString.ShouldStartWith("AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/");
            cosmosSettings?.DatabaseName.ShouldEqual("BookAppCosmos4");
        }

    }
}