using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Commands.IndexRecipes;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeById;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeByServingSize;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipesByIds;
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
    public class RecipeController : ApiControllerBase
    {

        private readonly ILogger<RecipeController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public RecipeController(ILogger<RecipeController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpPost("GetRecipes")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipes(List<int> recipesIdsToExclude)
        {
            GetRecipesQuery query = new GetRecipesQuery();
            query.ExcludeRecipes = recipesIdsToExclude;
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPost("Filter")]
        public async Task<ActionResult<List<RecipeDto>>> Filter(GetFilteredRecipesQuery query)
        {
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpGet("GetRecipeInServingSize")]
        public async Task<ActionResult<RecipeDto>> GetRecipeInServingSize(int recipeId, int servingSize)
        {
            GetRecipeByServingSizeQuery query = new GetRecipeByServingSizeQuery();
            query.RecipeId = recipeId;
            query.ServingSize = servingSize;
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpGet("GetRecipeById")]
        public async Task<ActionResult<RecipeDto>> GetRecipeById(int recipeId)
        {
            GetRecipeByIdQuery query = new GetRecipeByIdQuery();
            query.RecipeId = recipeId;
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPost("GetRecipesByIds")]
        public async Task<ActionResult<List<RecipeDto>>> GetRecipesByIds(List<int> recipeIds)
        {
            GetRecipesByIdsQuery query = new GetRecipesByIdsQuery();
            query.RecipeIds = recipeIds;
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPost("IndexRecipes")]
        public async Task<ActionResult> IndexRecipes()
        {
            IndexRecipesCommand command = new IndexRecipesCommand();
            await Mediator.Send(command);
            return Ok();
        }



    }
}
