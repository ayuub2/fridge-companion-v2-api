using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes;
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



        [HttpPost("Setup")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipes()
        {
            GetRecipesQuery command = new GetRecipesQuery();
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }


    }
}
