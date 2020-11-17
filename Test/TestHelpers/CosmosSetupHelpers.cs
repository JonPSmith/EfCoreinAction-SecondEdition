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
        private const string CosmosConnectionName = "CosmosConnectionString";

        private const string CosmosEmulatorCon =
            "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public static DbContextOptions<TContext> GetCosmosDbOptions<TContext>(this object callingClass)
            where TContext : DbContext
        {
            var config = AppSettings.GetConfiguration();
            var connectionString = config.GetConnectionString(CosmosConnectionName);
            var dbSettings = new CosmosDbSettings(connectionString, callingClass.GetType().Name);
            var builder = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName);

            //var builder2 = new DbContextOptionsBuilder
            //        <CosmosDbContext>()
            //    .UseCosmos(
            //        "connection string...",
            //        "MyCosmosDatabase");

            //var x = builder2.Options;

            return builder.Options;
        }

        public static (CosmosDbContext context, string databaseName)  GetCosmosDbAndDatabaseName(this object callingClass)
        {
            var config = AppSettings.GetConfiguration();
            var connectionString = config.GetConnectionString(CosmosConnectionName);
            var dbSettings = new CosmosDbSettings(connectionString, callingClass.GetType().Name);
            var builder = new DbContextOptionsBuilder<CosmosDbContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName);
            var cosmosContext = new CosmosDbContext(builder.Options);

            return (cosmosContext, dbSettings.DatabaseName);
        }


        public static (CosmosDbContext cosmosContext, Container Container) GetCosmosContextAndContainer(this object callingClass)
        {
            var config = AppSettings.GetConfiguration();
            var connectionString = config.GetConnectionString(CosmosConnectionName);
            var dbSettings = new CosmosDbSettings(connectionString, callingClass.GetType().Name);
            var builder = new DbContextOptionsBuilder<CosmosDbContext>()
                .UseCosmos(
                    dbSettings.ConnectionString,
                    dbSettings.DatabaseName);

            var cosmosContext = new CosmosDbContext(builder.Options);

            var cosmosClient = cosmosContext.Database.GetCosmosClient();
            var database = cosmosClient.GetDatabase(dbSettings.DatabaseName);
            var container = database.GetContainer(nameof(CosmosDbContext));

            return (cosmosContext, container);
        }
    }
}