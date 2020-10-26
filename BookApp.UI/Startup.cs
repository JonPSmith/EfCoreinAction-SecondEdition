// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Reflection;
using System.Text.Json.Serialization;
using BookApp.Infrastructure.Book.EventHandlers.ConcurrencyHandlers;
using BookApp.Infrastructure.Books.Seeding.AppStart;
using BookApp.Infrastructure.Orders.BizLogic.AppStart;
using BookApp.Persistence.EfCoreSql.Books;
using BookApp.Persistence.EfCoreSql.Orders;
using BookApp.Persistence.EfCoreSql.Orders.DbAccess.AppStart;
using BookApp.ServiceLayer.CachedSql.Books.AppStart;
using BookApp.ServiceLayer.DefaultSql.Books.AppStart;
using BookApp.ServiceLayer.EfCoreSql.Orders.AppStart;
using BookApp.ServiceLayer.UtfsSql.Books.AppStart;
using BookApp.UI.Logger;
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

            var connection = Configuration.GetConnectionString("DefaultConnection");

            //This registers both DbContext. Each MUST have a unique MigrationsHistoryTable for Migrations to work
            services.AddDbContext<BookDbContext>( 
                options => options.UseSqlServer(connection, dbOptions =>
                dbOptions.MigrationsHistoryTable("BookMigrationHistoryName")));
            services.AddDbContext<OrderDbContext>(
                options => options.UseSqlServer(connection, dbOptions =>
                    dbOptions.MigrationsHistoryTable("OrderMigrationHistoryName")));

            services.AddHttpContextAccessor();

            //This registers all the services across all the projects in this application
            services.RegisterOrdersDbAccess(Configuration);
            services.RegisterOrdersBizLogic(Configuration);
            services.RegisterBooksSeeding(Configuration);
            services.RegisterServiceLayerDefaultBooks(Configuration);
            services.RegisterServiceLayerUtfsSqlOrders(Configuration);
            services.RegisterServiceLayerCachedBooks(Configuration);
            services.RegisterServiceLayerDefaultOrders(Configuration);

            //Register EfCore.GenericEventRunner
            var eventConfig = new GenericEventRunnerConfig();
            eventConfig.RegisterSaveChangesExceptionHandler<BookDbContext>(BookWithEventsConcurrencyHandler.HandleCacheValuesConcurrency);
            eventConfig.AddActionToRunAfterDetectChanges<BookDbContext>(BookDetectChangesExtensions.ChangeChecker);
            services.RegisterGenericEventRunner(eventConfig,
                Assembly.GetAssembly(typeof(Infrastructure.Book.EventHandlers.ReviewAddedHandler))
                );

            //Register EfCoreGenericServices
            services.ConfigureGenericServicesEntities(typeof(BookDbContext), typeof(OrderDbContext))
                .ScanAssemblesForDtos(
                    Assembly.GetAssembly(typeof(ServiceLayer.DefaultSql.Books.Dtos.BookListDto))
                    ).RegisterGenericServices();
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