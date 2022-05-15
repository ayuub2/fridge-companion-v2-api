using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Fridge.Commands.AddIngredientToFridge;
using FridgeCompanionV2Api.Application.Fridge.Commands.UpdateFridgeItem;
using FridgeCompanionV2Api.Application.Fridge.Queries.GetAllFridgeItems;
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
    public class FridgeController : ApiControllerBase
    {

        private readonly ILogger<FridgeController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public FridgeController(ILogger<FridgeController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }



        [HttpPost()]
        public async Task<ActionResult<FridgeItemDto>> Create(AddIngredientToFridgeCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }


        [HttpGet()]
        public async Task<ActionResult<List<FridgeItemDto>>> Get()
        {
            var query = new GetAllFridgeItemsQuery();
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPut()]
        public async Task<ActionResult<FridgeItemDto>> Update(UpdateFridgeItemCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

    }
}
