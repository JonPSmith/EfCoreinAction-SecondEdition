// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading.Tasks;
using BookApp;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Test.Chapter05Listings
{
    public class ExampleProgram
    {
        public static async Task Main(string[] args) //#A
        {
            var host = CreateHostBuilder(args).Build(); //#B
            await host.MigrateDatabaseAsync(); //#C
            await host.RunAsync(); //#D
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                //see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/logging/?tabs=aspnetcore2x&view=aspnetcore-3.0#how-to-add-providers
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); //Clear logging providers to improve performance
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
    /**********************************************************
    #A You change the Main method to being async so that you can use async/await commands in your SetupDatabaseAsync method
    #B This call will run the Startup.Configure method, which sets up the DI services you need to setup/migrate your database
    #C this is where you call your extension method to migrate your database
    #D At the end you start the ASP.NET Core application.
    * ********************************************************/
}
