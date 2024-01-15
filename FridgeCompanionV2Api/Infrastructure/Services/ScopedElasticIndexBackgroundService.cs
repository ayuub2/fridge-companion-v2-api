using Amazon.Runtime.Internal.Util;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Ingredients.Commands.IndexIngredients;
using FridgeCompanionV2Api.Application.Recipes.Commands.IndexRecipes;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Infrastructure.Services
{
    public class ScopedElasticIndexBackgroundService : IScopedProcessingService
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ScopedElasticIndexBackgroundService> _logger;

        public ScopedElasticIndexBackgroundService(IMediator mediator, ILogger<ScopedElasticIndexBackgroundService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Indexing elastic search recipes and ingredients started.");
                await _mediator.Send(new IndexRecipesCommand());
                await _mediator.Send(new IndexIngredientsCommand());
                _logger.LogInformation("Indexing elastic search recipes and ingredients completed.");
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}
