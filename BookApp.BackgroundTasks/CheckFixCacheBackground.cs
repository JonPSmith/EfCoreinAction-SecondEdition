// Copyright (c) 2020 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using BookApp.Infrastructure.Books.EventHandlers;
using BookApp.Infrastructure.Books.EventHandlers.CheckFixCode;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BookApp.BackgroundTasks
{
    public class CheckFixCacheBackground : IHostedService, IDisposable
    {
        private readonly IServiceProvider _services;
        private readonly CheckFixCacheOptions _options;
        private readonly ILogger<CheckFixCacheBackground> _logger;
        private Timer _timer;

        private DateTime _ignoreBeforeDateUtc;

        public CheckFixCacheBackground(
            IServiceProvider services,
            IOptions<CheckFixCacheOptions> options,
            ILogger<CheckFixCacheBackground> logger)
        {
            _services = services;
            _options = options.Value;
            _logger = logger;

            _ignoreBeforeDateUtc = _options.IgnoreBeforeDateUtc 
                                   ?? DateTime.UtcNow;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CheckFixCacheBackground Hosted Service running.");

            _timer = new Timer(async _ => await DoWorkAsync(), null, TimeSpan.Zero,
                _options.WaitBetweenRuns);

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var checkService = scope.ServiceProvider
                    .GetRequiredService<ICheckFixCacheValuesService>();

                _ignoreBeforeDateUtc = await checkService.RunCheckAsync(_ignoreBeforeDateUtc);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("CheckFixCacheBackground Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}