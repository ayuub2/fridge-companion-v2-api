using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.BackgroudService
{
    public class NotificationService : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(10000);
            }

        }
    }
}
