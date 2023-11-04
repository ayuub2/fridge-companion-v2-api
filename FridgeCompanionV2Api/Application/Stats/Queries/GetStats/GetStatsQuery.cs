using Amazon.Runtime.Internal;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;

namespace FridgeCompanionV2Api.Application.Stats.Queries.GetStats
{
    public class GetStatsQuery : IRequest<StatDto>
    {
        public string UserId { get; set; }
        public string Duration { get; set; }
    }
}
