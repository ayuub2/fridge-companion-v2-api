using FridgeCompanionV2Api.Domain.Common;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IDomainEventService
    {
        Task Publish(DomainEvent domainEvent);
    }
}
