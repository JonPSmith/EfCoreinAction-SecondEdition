// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceLayer.DatabaseServices;
using ServiceLayer.DatabaseServices.Concrete;

namespace BookApp.HelperExtensions
{
    public static class DatabaseStartupHelpers
    {
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
                var context = services.GetRequiredService<EfCoreContext>();
                try
                {
                    var arePendingMigrations = context.Database.GetPendingMigrations().Any();
                    await context.Database.MigrateAsync();
                    if (arePendingMigrations)
                    {
                        await context.SeedDatabaseIfNoBooksAsync(env.WebRootPath);
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