// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using BookApp.UI.Controllers;
using Microsoft.Extensions.Configuration;

namespace BookApp.UI.Models
{
    public enum BookAppMenuSettings { Basic, SqlOnly, SqlAndCosmos, CosmosOnly, All}
    public class BookAppSettings
    {
        public string Title { get; set; }
        public BookAppMenuSettings MenuSet { get; set; }
        public string SqlConnectionString { get; set; }
        public string CosmosConnectionString { get; set; }
        public string CosmosDatabaseName { get; set; }

        public bool CosmosAvailable => CosmosConnectionString != null;

        public string GetDisplayControllerBasedOnTheMenuSet()
        {
            switch (MenuSet)
            {
                case BookAppMenuSettings.Basic:
                case BookAppMenuSettings.SqlOnly:
                    return "DefaultSql";
                case BookAppMenuSettings.SqlAndCosmos:
                case BookAppMenuSettings.CosmosOnly:
                case BookAppMenuSettings.All:
                    return "CosmosEf";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public static BookAppSettings GetBookAppSettings(IConfiguration config)
        {
            var setupNumString = config["SetupVersion"];
            if (setupNumString == null || !int.TryParse(setupNumString, out var versionNum))
                throw new InvalidOperationException("There must be a 'SetupVersion' integer in the appsettings.json file");

            return GetBookAppSettings(config, versionNum);
        }

        public static BookAppSettings GetBookAppSettings(IConfiguration config, int versionNum)
        {
            var settings = new BookAppSettings();
            config.GetSection($"Setup{versionNum}").Bind(settings);
            if (settings.Title == null)
                throw new InvalidOperationException($"Could not find 'Setup{versionNum}' section in appsettings.json file");

            return settings;
        }

        public override string ToString()
        {
            var cosmosString = CosmosAvailable
                ? $"CosmosConn = {CosmosConnectionString.Substring(0, 20)}..., DbName = {CosmosDatabaseName}"
                : "(no cosmos)";
            return $"{Title}: Menu = {MenuSet}, sqlCon = {SqlConnectionString.Substring(0, 20)}..., {cosmosString}";
        }
    }
}