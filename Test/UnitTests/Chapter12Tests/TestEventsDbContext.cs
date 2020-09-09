// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using BookApp.Domain.Books;
using GenericEventRunner.ForHandlers;
using GenericEventRunner.ForSetup;
using Microsoft.Extensions.DependencyInjection;
using Test.Chapter12Listings.BusinessLogic;
using Test.Chapter12Listings.EfClasses;
using Test.Chapter12Listings.EfCode;
using Test.Chapter12Listings.EventHandlers;
using Test.Chapter12Listings.EventInterfacesEtc;
using Test.Chapter12Listings.EventRunnerCode;
using Test.Chapter12Listings.Events;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests.Chapter12Tests
{
    public class TestEventsDbContext
    {
        [Fact]
        public void TestEventsDbContextSeededOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DomainEventsDbContext>();
            using var context = new DomainEventsDbContext(options);

            //ATTEMPT
            context.Database.EnsureCreated();

            //VERIFY
            context.SalesTaxes.Count().ShouldEqual(4);
            context.Locations.Count().ShouldEqual(3);
            int i = 1;
            foreach (var location in context.Locations)
            {
                location.Name.ShouldEqual($"Place{i++}");
                location.State.ShouldEqual("005");
            }
        }

        [Fact]
        public void TestAddEventManuallyOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DomainEventsDbContext>();
            using var context = new DomainEventsDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var location = context.Locations.First();
            location.State = "Test";

            //VERIFY
            var foundEvent = location.GetEventsThenClear().Single();
            foundEvent.ShouldBeType<LocationChangedEvent>();
            ((LocationChangedEvent)foundEvent).Location.State.ShouldEqual("Test");
            location.GetEventsThenClear().Count.ShouldEqual(0);
        }

        [Fact]
        public void TestNewQuoteCreatesEventOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<DomainEventsDbContext>();
            using var context = new DomainEventsDbContext(options);
            context.Database.EnsureCreated();

            //ATTEMPT
            var quote = new Quote(null);

            //VERIFY
            var foundEvent = quote.GetEventsThenClear().Single();
            foundEvent.ShouldBeType<QuoteLocationChangedEvent>();
            ((QuoteLocationChangedEvent)foundEvent).NewLocation.ShouldEqual(null);
        }

        [Fact]
        public void TestNewQuoteNoLocationGetsMaxSalesTaxOk()
        {
            //SETUP
            var context = SetupDependencyInjectionAndGetDbContext();
            context.Database.EnsureCreated();

            //ATTEMPT
            var quote  = new Quote(null);
            context.Add(quote);
            context.SaveChanges();

            //VERIFY
            quote.SalesTaxPercent.ShouldEqual(0.09);
        }

        [Fact]
        public void TestNewQuoteWithLocationGetCorrectSalesTaxOk()
        {
            //SETUP
            var context = SetupDependencyInjectionAndGetDbContext();
            context.Database.EnsureCreated();

            //ATTEMPT
            var quote = new Quote(context.Locations.First());
            context.Add(quote);
            context.SaveChanges();

            //VERIFY
            quote.SalesTaxPercent.ShouldEqual(0.05);
        }

        [Fact]
        public void TestChangeLocationEffectsQuotesLinkedOk()
        {
            //SETUP
            var context = SetupDependencyInjectionAndGetDbContext();
            context.Database.EnsureCreated();

            var locations = context.Locations.ToList();
            var quote1Loc0 = new Quote(locations[0]);
            var quote2Loc0 = new Quote(locations[0]);
            var quote3Loc1 = new Quote(locations[1]);
            context.AddRange(quote1Loc0, quote2Loc0, quote3Loc1);
            context.SaveChanges();

            //ATTEMPT
            locations.First().State = "004";
            context.SaveChanges();

            //VERIFY
            quote1Loc0.SalesTaxPercent.ShouldEqual(0.04);
            quote2Loc0.SalesTaxPercent.ShouldEqual(0.04);
            quote3Loc1.SalesTaxPercent.ShouldEqual(0.05);
        }

        [Fact]
        public void TestRegisterEventRunnerAndHandlersOk()
        {
            //SETUP
            var services = new ServiceCollection();

            //ATTEMPT
            services.RegisterEventRunnerAndHandlers(
                Assembly.GetAssembly(typeof(LocationChangedEventHandler)));

            //VERIFY
            services.Contains(new ServiceDescriptor(typeof(IEventRunner),
                typeof(EventRunner),
                ServiceLifetime.Scoped), new ServiceDescriptorCompare()).ShouldBeTrue();
            services.Contains(new ServiceDescriptor(typeof(IEventHandler<LocationChangedEvent>),
                typeof(LocationChangedEventHandler),
                ServiceLifetime.Transient), new ServiceDescriptorCompare()).ShouldBeTrue();
            services.Contains(new ServiceDescriptor(typeof(IEventHandler<LocationChangedEvent>),
                typeof(LocationChangedEventHandler),
                ServiceLifetime.Transient), new ServiceDescriptorCompare()).ShouldBeTrue();
        }

        private static DomainEventsDbContext SetupDependencyInjectionAndGetDbContext()
        {
            var services = new ServiceCollection();
            services.AddSingleton(SqliteInMemory.CreateOptions<DomainEventsDbContext>());
            services.AddScoped<DomainEventsDbContext>();

            services.AddScoped<IEventRunner, EventRunner>();                //#A

            services.AddTransient<IEventHandler<LocationChangedEvent>,      //#B
                    LocationChangedEventHandler>();                         //#B
            services.AddTransient<IEventHandler<QuoteLocationChangedEvent>, //#B
                QuoteLocationChangedEventHandler>();                        //#B

            services.AddTransient<ICalcSalesTaxService,                     //#C
                CalcSalesTaxService>();                                     //#C
            /********************************************************************
            #A Register the Event Runner which will be injected into your application's DbContext
            #B Registering all of your event handlers
            #C You need to register any services that your event handlers use
             *****************************************************************/

            var serviceProvider = services.BuildServiceProvider();
            var context = serviceProvider.GetRequiredService<DomainEventsDbContext>();
            return context;
        }
    }
}