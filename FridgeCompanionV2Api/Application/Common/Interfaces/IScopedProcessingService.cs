using System.Threading.Tasks;
using System.Threading;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }
}
