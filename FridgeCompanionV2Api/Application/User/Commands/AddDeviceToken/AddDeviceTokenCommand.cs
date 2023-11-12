using Amazon.Runtime.Internal;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;

namespace FridgeCompanionV2Api.Application.User.Commands.AddDeviceToken
{
    public class AddDeviceTokenCommand : IRequest<UserDto>
    {
        public string DeviceToken { get; set; }
        public string UserId { get; set; } 
    }
}
