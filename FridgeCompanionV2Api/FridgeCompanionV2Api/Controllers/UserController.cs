using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes;
using FridgeCompanionV2Api.Application.User.Commands.CreateUserProfile;
using FridgeCompanionV2Api.Application.User.Commands.UpdateUserProfile;
using FridgeCompanionV2Api.Application.User.Queries.GetUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : ApiControllerBase
    {

        private readonly ILogger<UserController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public UserController(ILogger<UserController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost()]
        public async Task<UserDto> Create(CreateUserProfileCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpPut()]
        public async Task<UserDto> Update(UpdateUserProfileCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpGet()]
        public async Task<UserDto> Get() 
        {
            var query = new GetUserProfileQuery();
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }
    }
}
