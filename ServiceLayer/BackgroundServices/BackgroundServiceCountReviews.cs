// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ServiceLayer.BackgroundServices
{
    public class BackgroundServiceCountReviews : BackgroundService //#A
    {
        private static TimeSpan _period = 
            new TimeSpan(0,1,0,0); //#B

        private readonly IServiceScopeFactory _scopeFactory; //#C
        private readonly ILogger<BackgroundServiceCountReviews> _logger;

        public BackgroundServiceCountReviews(IServiceScopeFactory scopeFactory, //#C
            ILogger<BackgroundServiceCountReviews> logger,
            TimeSpan periodOverride = default)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

            if (periodOverride != default)
                _period = periodOverride;
        }

        protected override async Task ExecuteAsync    //#D
            (CancellationToken stoppingToken)     //#D
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoWorkAsync(stoppingToken);
                await Task.Delay(_period, stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken) //#E
        {
            using (var scope = _scopeFactory.CreateScope()) //#F
            {
                var context = scope.ServiceProvider        //#G
                    .GetRequiredService<EfCoreContext>();  //#G
                var numReviews = await context.Set<Review>() //#H
                    .CountAsync(stoppingToken); //#H
                _logger.LogInformation(                             //#I
                    "Number of reviews: {numReviews}", numReviews); //#I
            }
        }
        /********************************************************************
        #A Inheriting the BackgroundService class means this class can run continuously in the background
        #B This holds the delay between the logging of the  
        #C The IServiceScopeFactory injects the DI service to create a newly created DI scope
        #D The BackgroundService class has a ExecuteAsync method you override to add your own code
        #E This is the method that the IHostedService will call when the set period has elapsed  
        #F This uses the ScopeProviderFactory to create a new DI scoped provider
        #G The DbContext instance created will be different to the ASP.NET Core version, as it's in a new scope
        #H This count of the reviews using an async method. You pass the stoppingToken to the async method because that is good practice 
        #I Finally you log the information 
         ********************************************************************/
    }
}