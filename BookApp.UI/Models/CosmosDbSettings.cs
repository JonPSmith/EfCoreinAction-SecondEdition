// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace BookApp.UI.Models
{
    /// <summary>
    /// Cosmos DB settings
    /// NOTE: Defaults to Azure Cosmos DB emulator
    /// </summary>
    public class CosmosDbSettings
    {
        public string EndPoint { get; set; } = "https://localhost:8081";

        public string AuthKey { get; set; } =
            "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public string DatabaseName { get; set; } = "BookApp3Cosmos";
    }
}