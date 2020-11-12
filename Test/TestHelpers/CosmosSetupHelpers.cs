// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using BookApp.UI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TestSupport.Helpers;

namespace Test.TestHelpers
{
    public static class CosmosSetupHelpers
    {
        public static DbContextOptions<TContext> GetCosmosDbOptions<TContext>(this object callingClass)
            where TContext : DbContext
        {
            var config = AppSettings.GetConfiguration();
            var dbSettings = new CosmosDbSettings();
            config.GetSection(nameof(CosmosDbSettings)).Bind(dbSettings);
            var builder = new DbContextOptionsBuilder<TContext>()
                .UseCosmos(
                    dbSettings.EndPoint,
                    dbSettings.AuthKey,
                    callingClass?.GetType().Name ?? dbSettings.DatabaseName);

            return builder.Options;
        }
    }
}