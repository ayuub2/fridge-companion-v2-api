using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Ingredients.Queries.GetAutoCompleteIngredients;
using FridgeCompanionV2Api.Application.Ingredients.Queries.GetIngredientsByName;
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
    [Route("[controller]")]
    public class IngredientController : ApiControllerBase
    {

        private readonly ILogger<IngredientController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public IngredientController(ILogger<IngredientController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpGet("AutoCompleteIngredient")]
        public async Task<ActionResult<List<IngredientDto>>> AutoCompleteIngredient(string ingredientName)
        {
            var query = new GetAutoCompleteIngredientsQuery();
            query.Query = ingredientName;
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPost("GetIngredientsByName")]
        public async Task<ActionResult<List<IngredientDto>>> GetIngredientsByName([FromBody] GetIngredientByNameQuery query)
        {
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }


    }
}
