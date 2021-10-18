using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Queries.GetUserProfile
{
    public class GetUserProfileQuery : IRequest<UserDto>
    {
        public string UserId { get; set; }
    }
}
