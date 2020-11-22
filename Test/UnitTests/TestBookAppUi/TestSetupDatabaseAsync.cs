// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.CosmosDb;
using BookApp.Infrastructure.Books.CosmosDb.EventsHandlers;
using BookApp.Infrastructure.Books.CosmosDb.Services;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.UI.HelperExtensions;
using GenericEventRunner.ForSetup;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Test.TestHelpers;
using TestSupport.Attributes;
using TestSupport.EfHelpers;
using TestSupport.Helpers;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.TestBookAppUi
{
    public class TestSetupDatabaseAsync
    {

        private class MyHost : IHost
        {
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public Task StartAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public Task StopAsync(CancellationToken cancellationToken = new CancellationToken())
            {
                throw new NotImplementedException();
            }

            public IServiceProvider Services { get; set; }
        }

        private class MyWebHostEnvironment : IWebHostEnvironment
        {
            public string ApplicationName { get; set; }
            public IFileProvider ContentRootFileProvider { get; set; }
            public string ContentRootPath { get; set; }
            public string EnvironmentName { get; set; }
            public string WebRootPath { get; set; }
            public IFileProvider WebRootFileProvider { get; set; }
        }


        [RunnableInDebugOnly]
        public async Task TestBookDbContextAddReviewCacheUpdatedOk()
        {
            //SETUP
            var booksOption = this.CreateUniqueClassOptions<BookDbContext>();
            var orderOption = this.CreateUniqueClassOptions<OrderDbContext>();
            var cosmosOption = this.GetCosmosDbOptions<CosmosDbContext>();
            var services = new ServiceCollection();

            services.RegisterGenericEventRunner(Assembly.GetAssembly(typeof(BookAddedHandlerAsync)));
            services.AddTransient<IBookToCosmosBookService, BookToCosmosBookService>();

            services.AddSingleton(booksOption);
            services.AddScoped<BookDbContext>();
            services.AddSingleton(orderOption);
            services.AddScoped<OrderDbContext>();
            services.AddSingleton(cosmosOption);
            services.AddScoped<CosmosDbContext>();
            services.AddSingleton<IWebHostEnvironment>(new MyWebHostEnvironment {WebRootPath = TestData.GetTestDataDir()});
            services.AddLogging();

            var serviceProvider = services.BuildServiceProvider();
            var host = new MyHost {Services = serviceProvider};

            using (var bookContext = new BookDbContext(booksOption))
            {
                bookContext.Database.EnsureDeleted();
            }
            using (var cosmosContext = new CosmosDbContext(cosmosOption))
            {
                await cosmosContext.Database.EnsureDeletedAsync();
            }

            //ATTEMPT
            await host.SetupDatabaseAsync();

            //VERIFY
            using (var bookContext = new BookDbContext(booksOption))
            {
                bookContext.Books.Count().ShouldEqual(6);
            }

        }
    }
}