using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteAllShoppingItems;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Commands.UpdateShoppingItem;
using FridgeCompanionV2Api.Application.ShoppingItems.Queries.GetShoppingItems;
using FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList;
using FridgeCompanionV2Api.Application.ShoppingLists.Queries.GetShoppingList;
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
    public class ShoppingController : ApiControllerBase
    {

        private readonly ILogger<ShoppingController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ShoppingController(ILogger<ShoppingController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("List")]
        public async Task<ActionResult<ShoppingListDto>> CreateList(CreateShoppingListCommand command)
        {
            // create list
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpGet("List")]
        public async Task<ActionResult<ShoppingListDto>> GetList()
        {
            GetShoppingListQuery query = new GetShoppingListQuery();
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpGet("Item")]
        public async Task<ActionResult<List<ShoppingItemDto>>> GetItems()
        {
            GetShoppingItemsCommand command = new GetShoppingItemsCommand();
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpPost("Item")]
        public async Task<ActionResult<ShoppingItemDto>> CreateItem(CreateShoppingItemCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpPut("Item")]
        public async Task<ActionResult<ShoppingItemDto>> UpdateItem(UpdateShoppingItemCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }

        [HttpDelete("Item")]
        public async Task<ActionResult<ShoppingItemDto>> DeleteItem(DeleteShoppingItemCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }


        [HttpDelete("ItemAll")]
        public async Task<ActionResult<List<ShoppingItemDto>>> DeleteItemAll(DeleteAllShoppingItemsCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }
    }
}
