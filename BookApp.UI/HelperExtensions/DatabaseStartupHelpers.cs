// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.UI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace BookApp.UI.HelperExtensions
{
    public static class DatabaseStartupHelpers
    {
        public static string GetCorrectSqlConnection(this IConfiguration config, BookAppSettings settings = null)
        {
            if (settings == null)
            {
                settings = new BookAppSettings();
                config.GetSection(nameof(BookAppSettings)).Bind(settings);
            }

            var connectionName = settings.ProductionDbs
                ? "Production-DefaultConnection" //Assumed to be in secrets
                : "DefaultConnection";
            var baseConnection = config.GetConnectionString(connectionName) ;

            if (baseConnection == null)
                throw new NullReferenceException($"The connection {connectionName} wasn't found.");

            if (settings.DbNameSuffix != null)
            {
                var builder = new SqlConnectionStringBuilder(baseConnection);
                builder["Initial Catalog"] += settings.DbNameSuffix;
                return builder.ConnectionString;
            }

            return baseConnection;
        }

        public static CosmosDbSettings GetCosmosDbSettings(this IConfiguration config, BookAppSettings settings = null)
        {
            if (settings == null)
            {
                settings = new BookAppSettings();
                config.GetSection(nameof(BookAppSettings)).Bind(settings);
            }

            var sectionName = settings.ProductionDbs
                ? "Production-CosmosDbSettings" //Assumed to be in secrets
                : "CosmosDbSettings";

            var result = new CosmosDbSettings();
            config.GetSection(sectionName).Bind(result);

            if (settings.DbNameSuffix != null)
            {
                result.DataBaseName += settings.DbNameSuffix;
            }

            return result;
        }

        /// <summary>
        /// This makes sure the database is created/updated
        /// </summary>
        /// <param name="webHost"></param>
        /// <returns></returns>
        public static async Task<IHost> SetupDatabaseAsync(this IHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IWebHostEnvironment>();
                var bookContext = services.GetRequiredService<BookDbContext>();
                var orderContext = services.GetRequiredService<OrderDbContext>();
                try
                {
                    await bookContext.Database.MigrateAsync();
                    await orderContext.Database.MigrateAsync();
                    if (!bookContext.Books.Any())
                    {
                        await bookContext.SeedDatabaseWithBooksAsync(env.WebRootPath);
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while creating/migrating or seeding the database.");

                    throw;
                }
            }

            return webHost;
        }

    }
}