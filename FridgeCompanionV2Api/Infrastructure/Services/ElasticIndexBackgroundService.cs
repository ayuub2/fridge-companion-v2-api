using FridgeCompanionV2Api.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Infrastructure.Services
{
    public class ElasticIndexBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ElasticIndexBackgroundService> _logger;

        public ElasticIndexBackgroundService(
           IServiceProvider serviceProvider,
           ILogger<ElasticIndexBackgroundService> logger) =>
           (_serviceProvider, _logger) = (serviceProvider, logger);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
            $"{nameof(ElasticIndexBackgroundService)} is running.");

            await DoWorkAsync(stoppingToken);

        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(ElasticIndexBackgroundService)} is working.");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IScopedProcessingService scopedProcessingService =
                    scope.ServiceProvider.GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.DoWorkAsync(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(ElasticIndexBackgroundService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
