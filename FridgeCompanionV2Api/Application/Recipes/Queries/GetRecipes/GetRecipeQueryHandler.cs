using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes
{
    public class GetRecipeQueryHandler : IRequestHandler<GetRecipesQuery, List<RecipeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRecipeService _recipeService;

        public GetRecipeQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetRecipesQuery> logger, IRecipeService recipeService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        }

        public async Task<List<RecipeDto>> Handle(GetRecipesQuery request, CancellationToken cancellationToken)
        {
            // recipe needs to take into consideration the user profile, user fridge and return 10 items
            // check if they have user profile otherwise ignore preferences
            // check their fridge contents and compare them with recipe ingredients

            // TODO: Implement isNutFree

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = _applicationDbContext.Users.Include(x => x.UserDiets).ThenInclude(x => x.DietType).FirstOrDefault(x => x.Id == request.UserId);
            var isAllergicNuts = false;
            var isGlutenFree = false;
            List<DietType> userDiets = new List<DietType>();

            if (user is not null) 
            {
                isAllergicNuts = user.IsAllergicNuts;
                isGlutenFree = user.IsGlutenFree;
                userDiets = user.UserDiets.Select(x => x.DietType).ToList();
            }

            var items = _applicationDbContext.FreshFridgeItems(request.UserId).ToList();

            var recipeEntites = _applicationDbContext.Recipes
                .Include(x => x.Ingredients)
                    .ThenInclude(x => x.Ingredient)
                        .ThenInclude(x => x.Location)
                .Include(x => x.Ingredients).ThenInclude(x => x.Measurement)
                .Include(x => x.Ingredients).ThenInclude(i => i.Ingredient).ThenInclude(id => id.GroupTypes).ThenInclude(idt => idt.IngredientGroupType)
                .Include(x => x.Ingredients).ThenInclude(i => i.Ingredient).ThenInclude(id => id.DietTypes).ThenInclude(idt => idt.Diet)
                .Include(x => x.DishTypes)
                    .ThenInclude(x => x.Dish)
                .Include(x => x.RecipeSteps)
                .Include(x => x.CuisineTypes)
                    .ThenInclude(x => x.Cuisine).AsNoTracking()
                .Where(x => !x.IsDeleted);

            
            recipeEntites = _recipeService.ExcludeRecipes(request.ExcludeRecipes, recipeEntites);

            var recipes = _mapper.Map<List<RecipeDto>>(recipeEntites.ToList());

            if (isGlutenFree) recipes = _recipeService.FilterGlutenRecipes(recipes);


            // we need reicpes where all the ingredients have all the diets specified by the user
            if (userDiets.Any())
            {
                recipes = _recipeService.FilterDiets(userDiets.Select(x => x.Id).ToList(), recipes);     
            }

            if (items.Any())
            {
                foreach (var recipe in recipes)
                {
                    var numberOfUsedIngredient = recipe.Ingredients.Any(x => items.Any(i => i.IngredientId == x.Ingredient.Id)) ? CalculateNumberOfUsedIngredients(recipe.Ingredients, items) : 0;
                    recipe.NumberOfUsedIngredients = numberOfUsedIngredient;
                }
                var topTenRecipes = recipes.OrderByDescending(x => x.NumberOfUsedIngredients).Take(10).ToList();
                // Add padding with random recipes that conform to users dietary needs
                while (topTenRecipes.Count < 10)
                {
                    var randomRecipe = GetRandomRecipe(recipes.Except(topTenRecipes).ToList());
                    topTenRecipes.Add(randomRecipe);
                }
                return topTenRecipes;
            }

            //TODO: Add exclude
            return recipes.Take(10).ToList();
        }

        private RecipeDto GetRandomRecipe(List<RecipeDto> recipes)
        {
            var random = new Random();
            int index = random.Next(recipes.Count);
            return recipes[index];
        }

        private int CalculateNumberOfUsedIngredients(List<RecipeIngredientDto> recipeIngredients, List<FridgeItem> fridgeIngredients)
        {
            var numberOfUsedIngredients = 0;
            // For each recipe ingredient
            foreach (var recipeIngredient in recipeIngredients)
            {
                var fridgeIngredient = fridgeIngredients.FirstOrDefault(x => x.IngredientId == recipeIngredient.Ingredient.Id);
                if (fridgeIngredient is not null)
                {
                    // if its the same measurment type its easy to compare of the user has enough in the fridge
                    if (fridgeIngredient.MeasurementId == recipeIngredient.Measurement.Id && fridgeIngredient.Amount > recipeIngredient.Amount)
                    {
                        numberOfUsedIngredients++;
                    }

                    if (fridgeIngredient.MeasurementId != recipeIngredient.Measurement.Id)
                    {
                        // if its not the same we must get the average grams per unit of that measurement type. This is for both fridge and recipe ingredient. We then multiple average gram and amount to get overall amount in grams for that item
                        // we then check if the user has more in the fridge
                        var recipeIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == recipeIngredient.Ingredient.Id && x.MeasurementId == recipeIngredient.Measurement.Id);
                        var fridgeIngredientConverter = _applicationDbContext.IngredientMeasurements.FirstOrDefault(x => x.IngredientId == fridgeIngredient.IngredientId && x.MeasurementId == fridgeIngredient.MeasurementId);
                        decimal recipeIngredientAmountGrams = 0;
                        decimal fridgeIngredientAmountGrams = 0;
                        if (recipeIngredientConverter.AverageGrams is not null)
                        {
                            recipeIngredientAmountGrams = recipeIngredientConverter.AverageGrams.Value * Convert.ToDecimal(recipeIngredient.Amount);
                        }
                        else if (recipeIngredient.Measurement.Name == "Grams")
                        {
                            recipeIngredientAmountGrams = recipeIngredient.Amount;
                        }
                        else
                        {
                            _logger.LogError("Unable to convert");
                            throw new Exception($"Unable to convert measurement to grams - {recipeIngredient.Measurement.Id}");
                        }

                        if (fridgeIngredientConverter.AverageGrams is not null)
                        {
                            fridgeIngredientAmountGrams = fridgeIngredientConverter.AverageGrams.Value * Convert.ToDecimal(fridgeIngredient.Amount);
                        }
                        else if (fridgeIngredient.Measurement.Name == "Grams")
                        {
                            fridgeIngredientAmountGrams = fridgeIngredient.Amount;
                        }
                        else
                        {
                            _logger.LogError("Unable to convert");
                            throw new Exception($"Unable to convert measurement to grams - {fridgeIngredient.MeasurementId}");
                        }

                        if (fridgeIngredientAmountGrams > recipeIngredientAmountGrams)
                        {
                            numberOfUsedIngredients++;
                        }
                    }
                }
            }
            return numberOfUsedIngredients;
        }
    }
}
