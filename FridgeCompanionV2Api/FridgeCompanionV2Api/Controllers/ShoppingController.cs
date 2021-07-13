using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList;
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
    public class ShoppingController : ApiControllerBase
    {

        private readonly ILogger<ShoppingController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ShoppingController(ILogger<ShoppingController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<ActionResult<ShoppingListDto>> Create(CreateShoppingListCommand command)
        {
            command.UserId = _currentUserService.UserId;
            return await Mediator.Send(command);
        }
    }
}
