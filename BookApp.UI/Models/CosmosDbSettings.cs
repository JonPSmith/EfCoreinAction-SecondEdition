// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace BookApp.UI.Models
{
    /// <summary>
    /// Cosmos DB settings
    /// NOTE: Defaults to Azure Cosmos DB emulator
    /// </summary>
    public class CosmosDbSettings
    {
        public CosmosDbSettings(string connectionString, string databaseName)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DatabaseName = databaseName ?? throw new ArgumentNullException(nameof(databaseName));
        }

        public string ConnectionString { get; }

        public string DatabaseName { get; }
    }
}