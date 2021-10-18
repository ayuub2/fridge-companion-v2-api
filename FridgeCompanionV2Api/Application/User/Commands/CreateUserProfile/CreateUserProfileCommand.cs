using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.CreateUserProfile
{
    public class CreateUserProfileCommand : IRequest<UserDto>
    {
        public string UserId { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsAllergicNuts { get; set; }
        public List<DietTypeDto> Diets { get; set; }
    }
}
