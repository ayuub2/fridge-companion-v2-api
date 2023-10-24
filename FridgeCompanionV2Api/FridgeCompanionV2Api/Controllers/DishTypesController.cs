   using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.CuisineTypes.Queries.GetCuisineTypes;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteAllShoppingItems;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.UpdateShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Queries.GetShoppingItems;
using FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList;
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
    public class DishTypesController : ApiControllerBase
    {

        private readonly ILogger<DishTypesController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public DishTypesController(ILogger<DishTypesController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet()]
        public async Task<ActionResult<List<DishTypeDto>>> GetItems()
        {
            GetIngredientGroupTypes query = new GetIngredientGroupTypes();
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }
    }
}
