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

        public BackgroundServiceCountReviews(
            IServiceScopeFactory scopeFactory, //#C
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
            while (!stoppingToken.IsCancellationRequested) //#E
            {
                await DoWorkAsync(stoppingToken);
                await Task.Delay(_period, stoppingToken);
            }
        }
        /********************************************************************
        #A Inheriting the BackgroundService class means this class can run continuously in the background
        #B This holds the delay between each call to the code to log the number of reviews  
        #C The IServiceScopeFactory injects the DI service to that you use to create a new DI scope
        #D The BackgroundService class has a ExecuteAsync method you override to add your own code
        #E This loop repeatably calls the DoWorkAsync method, with a delay until the next call is made
         ********************************************************************/

        private async Task DoWorkAsync(CancellationToken stoppingToken) //#A
        {
            using (var scope = _scopeFactory.CreateScope()) //#B
            {
                var context = scope.ServiceProvider        //#C
                    .GetRequiredService<EfCoreContext>();  //#C
                var numReviews = await context.Set<Review>() //#D
                    .CountAsync(stoppingToken);                 //#D
                _logger.LogInformation(                             //#E
                    "Number of reviews: {numReviews}", numReviews); //#E
            }
        }
    }
    /********************************************************************
    #A This is the method that the IHostedService will call when the set period has elapsed  
    #B This uses the ScopeProviderFactory to create a new DI scoped provider
    #C Because of the scoped DI provider this DbContext instance created will be different to all the other instances of the DbContext
    #D This count of the reviews using an async method. You pass the stoppingToken to the async method because that is good practice 
    #E Finally you log the information 
     ********************************************************************/
}