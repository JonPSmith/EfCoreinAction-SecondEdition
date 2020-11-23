// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.Infrastructure.AppParts
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

        public override string ToString()
        {
            var cosmosString = CosmosAvailable
                ? $"CosmosConn = {CosmosConnectionString.Substring(0, 20)}..., DbName = {CosmosDatabaseName}"
                : "(no cosmos)";
            return $"{Title}: Menu = {MenuSet}, sqlCon = {SqlConnectionString.Substring(0, 20)}..., {cosmosString}";
        }
    }
}