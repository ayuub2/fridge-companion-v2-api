using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using System.Linq;
using System;
using AutoMapper;
using System.Collections.Generic;
using FridgeCompanionV2Api.Application.Common.Models;

namespace FridgeCompanionV2Api.BackgroudService
{
    public interface IScopedProcessingService
    {
        Task DoWorkAsync(CancellationToken stoppingToken);
    }

    public sealed class ScopedNotificationService : IScopedProcessingService
    {
        private readonly ILogger<ScopedNotificationService> _logger;
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IRecipeService _recipeService;
        private readonly IMapper _mapper;

        public ScopedNotificationService(
            ILogger<ScopedNotificationService> logger, IApplicationDbContext applicationDbContext, IRecipeService recipeService, IMapper mapper)
        {
            _logger = logger;
            _applicationDbContext = applicationDbContext;
            _recipeService = recipeService;
            _mapper = mapper;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                _logger.LogInformation(
                    "{ServiceName} working}",
                    nameof(ScopedNotificationService));

                var usersWithTokens = _applicationDbContext.Users.Where(x => !string.IsNullOrEmpty(x.DeviceToken));
                var recipes = _mapper.Map<List<RecipeDto>>(_applicationDbContext.GetRecipesWithDetails().ToList());
                foreach (var user in usersWithTokens) 
                {
                    var freshItems = _applicationDbContext.FreshFridgeItems(user.Id);
                    var userItemsAboutToExpire = freshItems.Where(x => (x.Expiration - DateTime.UtcNow).TotalDays <= 3);
                    if(userItemsAboutToExpire.Any())
                    {
                        var recipe = _recipeService.OrderRecipesByIngredients(freshItems.Select(x => x.IngredientId).ToList(), recipes).FirstOrDefault();
                    }
                    
                }

                await Task.Delay(10_000, stoppingToken);
            }
        }
    }
}
