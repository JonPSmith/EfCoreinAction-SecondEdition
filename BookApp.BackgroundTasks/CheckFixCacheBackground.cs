// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.CachedValues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookApp.BackgroundTasks
{
    public class CheckFixCacheBackground : BackgroundService
    {
        private CancellationToken _stopCancellationToken = new CancellationToken();
        private readonly NightlyTimer _nightlyTimer = new NightlyTimer();

        private readonly IServiceProvider _services;
        private readonly ILogger<CheckFixCacheBackground> _logger;


        public CheckFixCacheBackground(
            IServiceProvider services,
            ILogger<CheckFixCacheBackground> logger)
        {
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CheckFixCacheBackground Hosted Service running.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delayTime = _nightlyTimer.TimeToWait();
                await Task.Delay(delayTime, stoppingToken);
                if (!_stopCancellationToken.IsCancellationRequested)
                    await DoWorkAsync();
            }
        }

        private async Task DoWorkAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var checkService = scope.ServiceProvider
                    .GetRequiredService<ICheckFixCacheValuesService>();

                var last25Hours = DateTime.UtcNow.AddHours(-25);
                await checkService.RunCheckAsync(last25Hours, true, _stopCancellationToken);
                _logger.LogDebug("Ran the CheckFixCacheValuesService.");
            }
        }

        public override Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CheckFixCacheBackground Hosted Service is stopping.");

            _stopCancellationToken = new CancellationToken(true);

            return Task.CompletedTask;
        }

    }
}