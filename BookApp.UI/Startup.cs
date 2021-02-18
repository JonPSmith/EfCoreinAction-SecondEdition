// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Reflection;
using System.Text.Json.Serialization;
using AutoMapper.Configuration.Annotations;
using BookApp.BackgroundTasks;
using BookApp.BizLogic.Orders.Orders;
using BookApp.Infrastructure.AppParts;
using BookApp.Infrastructure.Books.CachedValues;
using BookApp.Infrastructure.Books.CachedValues.ConcurrencyHandlers;
using BookApp.Infrastructure.Books.CachedValues.EventHandlers;
using BookApp.Infrastructure.Books.CosmosDb;
using BookApp.Infrastructure.Books.CosmosDb.EventsHandlers;
using BookApp.Infrastructure.Books.Seeding;
using BookApp.Persistence.CosmosDb.Books;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess;
using BookApp.ServiceLayer.CachedSql.Books;
using BookApp.ServiceLayer.CosmosEf.Books;
using BookApp.ServiceLayer.DefaultSql.Books;
using BookApp.ServiceLayer.DefaultSql.Books.Dtos;
using BookApp.ServiceLayer.DisplayCommon.Books.Dtos;
using BookApp.ServiceLayer.EfCoreSql.Orders.OrderServices;
using BookApp.ServiceLayer.UdfsSql.Books;
using BookApp.UI.HelperExtensions;
using BookApp.UI.Logger;
using BookApp.UI.Models;
using BookApp.UI.Services;
using GenericEventRunner.ForSetup;
using GenericServices.Setup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCore.AutoRegisterDi;
using SoftDeleteServices.Configuration;

namespace BookApp.UI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) //#A
        {
            services.AddControllersWithViews() //#B
                .AddRazorRuntimeCompilation() //This recompile a razor page if you edit it while the app is running
                //Added this because my logs display needs the enum as a string
                .AddJsonOptions(opts =>
                {
                    opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            var bookAppSettings = Configuration.GetBookAppSettings();
            services.AddSingleton(bookAppSettings);

            //This gets the correct sql connection string based on the BookAppSettings
            var sqlConnection = Configuration.GetCorrectSqlConnection(bookAppSettings);

            //This registers both DbContext. Each MUST have a unique MigrationsHistoryTable for Migrations to work
            services.AddDbContext<BookDbContext>( 
                options => options.UseSqlServer(sqlConnection, dbOptions =>
                dbOptions.MigrationsHistoryTable("BookMigrationHistoryName")));
            services.AddDbContext<OrderDbContext>(
                options => options.UseSqlServer(sqlConnection, dbOptions =>
                    dbOptions.MigrationsHistoryTable("OrderMigrationHistoryName")));

            var cosmosSettings = Configuration.GetCosmosDbSettings(bookAppSettings);
            if (cosmosSettings != null)
                services.AddDbContext<CosmosDbContext>(options => options.UseCosmos(
                    cosmosSettings.ConnectionString,
                    cosmosSettings.DatabaseName));
            else
            {
                services.AddSingleton<CosmosDbContext>(_ => null);
            }

            services.AddHttpContextAccessor();

            services.Configure<BookAppSettings>(options => 
                Configuration.GetSection(nameof(BookAppSettings)).Bind(options));
            services.AddSingleton<IMenuBuilder, MenuBuilder>();

            //This registers all the services across all the projects in this application
            var diLogs = services.RegisterAssemblyPublicNonGenericClasses(
                    Assembly.GetAssembly(typeof(ICheckFixCacheValuesService)),
                    Assembly.GetAssembly(typeof(BookListDto)),
                    Assembly.GetAssembly(typeof(IBookToCosmosBookService)),
                    Assembly.GetAssembly(typeof(IBookGenerator)),
                    Assembly.GetAssembly(typeof(IPlaceOrderBizLogic)),
                    Assembly.GetAssembly(typeof(IPlaceOrderDbAccess)),
                    Assembly.GetAssembly(typeof(IListBooksCachedService)),
                    Assembly.GetAssembly(typeof(ICosmosEfListNoSqlBooksService)),
                    Assembly.GetAssembly(typeof(IListBooksService)),
                    Assembly.GetAssembly(typeof(IDisplayOrdersService)),
                    Assembly.GetAssembly(typeof(IListUdfsBooksService)),
                    Assembly.GetAssembly(typeof(IListUdfsBooksService))
                )
                .AsPublicImplementedInterfaces();

            services.AddHostedService<CheckFixCacheBackground>();

            //Register EfCore.GenericEventRunner
            var eventConfig = new GenericEventRunnerConfig
            {
                NotUsingDuringSaveHandlers = cosmosSettings == null //This stops any attempts to update cosmos db if not turned on
            };
            eventConfig.RegisterSaveChangesExceptionHandler<BookDbContext>(BookWithEventsConcurrencyHandler.HandleCacheValuesConcurrency);
            eventConfig.AddActionToRunAfterDetectChanges<BookDbContext>(BookDetectChangesExtensions.ChangeChecker);
            var logs = services.RegisterGenericEventRunner(eventConfig,
                Assembly.GetAssembly(typeof(ReviewAddedHandler)),   //SQL cached values event handlers
                Assembly.GetAssembly(typeof(BookChangeHandlerAsync))  //Cosmos Db event handlers
                );

            //Register EfCoreGenericServices
            services.ConfigureGenericServicesEntities(typeof(BookDbContext), typeof(OrderDbContext))
                .ScanAssemblesForDtos(
                    Assembly.GetAssembly(typeof(BookListDto)),
                    Assembly.GetAssembly(typeof(AddReviewDto))
                ).RegisterGenericServices();

            var softLogs = services.RegisterSoftDelServicesAndYourConfigurations();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory, IHttpContextAccessor httpContextAccessor)
        {
            loggerFactory.AddProvider(new RequestTransientLogger(() => httpContextAccessor));
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}