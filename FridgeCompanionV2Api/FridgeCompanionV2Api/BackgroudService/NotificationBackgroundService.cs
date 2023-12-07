using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.BackgroudService
{
    public class NotificationBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<NotificationBackgroundService> _logger;

        public NotificationBackgroundService(
           IServiceProvider serviceProvider,
           ILogger<NotificationBackgroundService> logger) =>
           (_serviceProvider, _logger) = (serviceProvider, logger);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
            $"{nameof(NotificationBackgroundService)} is running.");

            await DoWorkAsync(stoppingToken);

        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(NotificationBackgroundService)} is working.");

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
                $"{nameof(NotificationBackgroundService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
