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
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : ApiControllerBase
    {

        private readonly ILogger<RecipeController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public RecipeController(ILogger<RecipeController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpGet("GetRecipes")]
        public async Task<ActionResult<List<RecipeDto>>> GetIGetRecipestems()
        {
            GetRecipesQuery command = new GetRecipesQuery();
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }


    }
}
