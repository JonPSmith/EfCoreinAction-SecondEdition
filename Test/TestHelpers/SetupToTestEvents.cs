// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GenericEventRunner.ForDbContext;
using GenericEventRunner.ForHandlers;
using GenericEventRunner.ForSetup;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using TestSupport.EfHelpers;

namespace Test.TestHelpers
{
    public static class SetupToTestEvents
    {

        /// <summary>
        /// This extension method provides a way to set up a DbContext with an EventsRunner and also registers all
        /// the event handlers in the assembly that the TRunner class is in. 
        /// </summary>
        /// <typeparam name="TContext">Your DbContext type</typeparam>
        /// <typeparam name="THandler">The type of one of your event handlers.
        /// The whole assembly that the TRunner is in will be scanned for event handlers</typeparam>
        /// <param name="options">The <code>T:DbContextOptions{TContext}</code> for your DbContext</param>
        /// <param name="logs">Optional. If provided the it uses the EfCore.TestSupport logging provider to return logs</param>
        /// <param name="config">Optional. Allows you to change the configuration setting for GenericEventRunner</param>
        /// <returns>An instance of the DbContext created by DI and therefore containing the EventsRunner</returns>
        public static TContext CreateDbWithDiForHandlers<TContext, THandler>(this DbContextOptions<TContext> options,
            List<LogOutput> logs = null, IGenericEventRunnerConfig config = null) where TContext : DbContext where THandler : class
        {
            var services = new ServiceCollection();
            if (logs != null)
            {
                services.AddSingleton<ILogger<EventsRunner>>(new Logger<EventsRunner>(new LoggerFactory(new[] { new MyLoggerProvider(logs) })));
            }
            else
            {
                services.AddSingleton<ILogger<EventsRunner>>(new NullLogger<EventsRunner>());
            }

            var assembliesToScan = new Assembly[]
            {
                Assembly.GetAssembly(typeof(THandler)),
                Assembly.GetExecutingAssembly()         //This will pick up any event handlers in your unit tests assembly
            };

            services.RegisterGenericEventRunner(config ?? new GenericEventRunnerConfig(), assembliesToScan);
            services.AddSingleton(options);
            services.AddScoped<TContext>();
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TContext>();
            return context;
        }

        /// <summary>
        /// This registers the GenericEventRunner parts, plus the list of specific event handlers you have provided
        /// It then creates the given TContext which should use the GenericEventRunner
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="options"></param>
        /// <param name="logs"></param>
        /// <param name="eventHandlers"></param>
        /// <returns></returns>
        public static TContext CreateDbWithDiForHandlers<TContext>(this DbContextOptions<TContext> options,
            List<LogOutput> logs, params Type[] eventHandlers) where TContext : DbContext 
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILogger<EventsRunner>>(
                new Logger<EventsRunner>(new LoggerFactory(new[] {new MyLoggerProvider(logs)})));
            services.AddSingleton<IGenericEventRunnerConfig>(new GenericEventRunnerConfig());
            services.AddScoped<IEventsRunner, EventsRunner>();

            foreach (var eventHandler in eventHandlers)
            {
                var typeInterface = eventHandler.GetInterfaces().Single();
                services.AddTransient(typeInterface ,eventHandler);
            }

            services.AddSingleton(options);
            services.AddScoped<TContext>();
            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<TContext>();
            return context;
        }
    }
}