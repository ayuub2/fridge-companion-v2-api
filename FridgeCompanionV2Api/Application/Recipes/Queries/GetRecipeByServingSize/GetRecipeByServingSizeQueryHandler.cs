using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeByServingSize
{
    public class GetRecipeByServingSizeQueryHandler : IRequestHandler<GetRecipeByServingSizeQuery, RecipeDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRecipeService _recipeService;
        private readonly IConverterService _converterService;

        public GetRecipeByServingSizeQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetRecipeByServingSizeQueryHandler> logger, IRecipeService recipeService, IConverterService converterService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService)); ;
            _converterService = converterService ?? throw new ArgumentNullException(nameof(converterService));
        }
        public async Task<RecipeDto> Handle(GetRecipeByServingSizeQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipe = _applicationDbContext.GetRecipesWithDetails().FirstOrDefault(x => x.Id == request.RecipeId);

            if (recipe is null) throw new NotFoundException("Recipe not found.");

            var recipeDto = _mapper.Map<RecipeDto>(recipe);
            recipeDto = _recipeService.CalculateNutrition(request.ServingSize, recipeDto);
            return _recipeService.OrderRecipesByIngredients(_applicationDbContext.FreshFridgeItems(request.UserId).Select(x => x.IngredientId).ToList(), new List<RecipeDto>() { recipeDto }).FirstOrDefault();
         
        }
    }
}
