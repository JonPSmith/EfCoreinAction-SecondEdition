// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading.Tasks;
using BookApp;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test.Chapter05Listings
{
    public static class ExampleMigrateDatabase
    {
        //see https://github.com/aspnet/EntityFrameworkCore/issues/9033#issuecomment-317104564
        public static async Task MigrateDatabaseAsync
            (this IHost webHost) //#A
        {
            using (var scope = webHost.Services.CreateScope()) //#B
            {
                var services = scope.ServiceProvider;    //#C
                using (var context = services            //#C
                    .GetRequiredService<EfCoreContext>())//#C
                {
                    try
                    {
                        await context.Database.MigrateAsync(); //#D
                        //Put any complex database seeding here //#E
                    }
                    catch (Exception ex) //#F
                    {
                        var logger = services
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex,
                        "An error occurred while migrating the database.");

                        throw; //#G
                    }
                }
            }
        }

        /******************************************************
        #A I create an extension method that takes in IHost
        #B This creates a scoped service provider. Once the using block is left then all the services will be unavailable. This is the recommended way to obtain services outside of an HTTP request
        #C This creates an instance of the application's DbContext that only has a lifetime of the outer using statement
        #D Then I call EF Core's Migrate command to apply any outstanding migrations at startup.
        #E You can add a method here to handle complex seeding of the database if required
        #F If there is an exception I log the information so that I can diagnose it. 
        #G I rethrow the exception because I don't want the application to carry on if there was a problem with migrating the database
            * ****************************************************/
    }
}