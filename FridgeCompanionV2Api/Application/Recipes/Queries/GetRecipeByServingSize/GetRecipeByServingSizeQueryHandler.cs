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

            // foreach ingredient in the recipe we convert the nutritional values are relevant to the serving size of the recipe
            foreach (var ingredient in recipeDto.Ingredients)
            {
                // we first convert the ingredient amount from any measurement type to grams. This will allow us to compare to the
                // ingredient standard which is in grams and is used to determine the nutritional values saved in the DB
                var ingredientGrams = _converterService.ConvertIngredientAmountToGrams(ingredient);
                var gramFactor = ingredientGrams / ingredient.Ingredient.Standard;
                var ingredientDto = ingredient.Ingredient;
                ingredientDto.Calories = decimal.ToInt32(ingredientDto.Calories * gramFactor);
                ingredientDto.Protein = ingredientDto.Protein * gramFactor;
                ingredientDto.Fat = ingredientDto.Fat * gramFactor; 
                ingredientDto.Fibre = ingredientDto.Fibre * gramFactor; 
                ingredientDto.Carb = ingredientDto.Carb * gramFactor; 
                ingredientDto.Sugar = ingredientDto.Sugar * gramFactor; 
            }


            var recipeInServingSize = _recipeService.GetRecipeInServingSize(request.ServingSize, recipeDto, _applicationDbContext, _mapper);

            recipeInServingSize.Nutrition = new NutritionDto() 
            {
                Calories = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Calories),
                Protein = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Protein),
                Fat = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fat),
                Fibre= recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fibre),
                Carb = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Carb),
                Sugar = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Sugar)
            };

            return _recipeService.OrderRecipesByIngredients(_applicationDbContext.FreshFridgeItems(request.UserId).Select(x => x.IngredientId).ToList(), new List<RecipeDto>() { recipeInServingSize }).FirstOrDefault();
         
        }
    }
}
