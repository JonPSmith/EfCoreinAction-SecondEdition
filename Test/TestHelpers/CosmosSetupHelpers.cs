// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.Persistence.CosmosDb.Books;
using BookApp.UI.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.Helpers;

namespace Test.TestHelpers
{
    public static class CosmosSetupHelpers
    {
        private const string CosmosEmulatorCon =
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public static DbContextOptions<TContext> GetCosmosDbOptions<TContext>(this object callingClass)
            where TContext : DbContext
        {
            var config = AppSettings.GetConfiguration();
            var dbSettings = new CosmosDbSettings(CosmosEmulatorCon, callingClass.GetType().Name);
            config.GetSection(nameof(CosmosDbSettings)).Bind(dbSettings);
            var builder = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName);

            return builder.Options;
        }


        public static (CosmosDbContext cosmosContext, Container Container) GetCosmosContextAndContainer(this object callingClass)
        {
            var config = AppSettings.GetConfiguration();
            var dbSettings = new CosmosDbSettings(CosmosEmulatorCon, callingClass.GetType().Name);
            config.GetSection(nameof(CosmosDbSettings)).Bind(dbSettings);
            var databaseName = callingClass?.GetType().Name ?? dbSettings.DatabaseName;
            var builder = new DbContextOptionsBuilder<CosmosDbContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName);

            var cosmosContext = new CosmosDbContext(builder.Options);

            var cosmosClient = cosmosContext.Database.GetCosmosClient();
            var database = cosmosClient.GetDatabase(databaseName);
            var container = database.GetContainer(nameof(CosmosDbContext));

            return (cosmosContext, container);
        }
    }
}