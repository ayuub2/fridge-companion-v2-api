using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Receipt.Queries.ScanReceipt;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes;
using FridgeCompanionV2Api.Application.User.Commands.AddFavouriteRecipe;
using FridgeCompanionV2Api.Application.User.Commands.AddMadeRecipe;
using FridgeCompanionV2Api.Application.User.Commands.CreateUserProfile;
using FridgeCompanionV2Api.Application.User.Commands.DeleteFavouriteRecipe;
using FridgeCompanionV2Api.Application.User.Commands.DeleteMadeRecipe;
using FridgeCompanionV2Api.Application.User.Commands.RemoveRecipeIngredientsFromFridge;
using FridgeCompanionV2Api.Application.User.Commands.UpdateUserProfile;
using FridgeCompanionV2Api.Application.User.Queries.GetUserProfile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FridgeCompanionV2Api.Application.Receipt.Queries.ScanBarcode;

namespace FridgeCompanionV2Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ScanningController : ApiControllerBase
    {

        private readonly ILogger<ScanningController> _logger;
        private readonly ICurrentUserService _currentUserService;

        public ScanningController(ILogger<ScanningController> logger, ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("ScanReceipt")]
        public async Task<ScanReceiptDto> ScanReceipt([FromForm] IFormFile file)
        {
            var query = new ScanReceiptQuery() { Image = file };
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }

        [HttpPost("ScanBarcode")]
        public async Task<ScanBarcodeDto> ScanBarcode(ScanBarcodeQuery query)
        {
            query.UserId = _currentUserService.UserId;
            return await Mediator.Send(query);
        }
    }
}
