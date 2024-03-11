using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzyString;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.CommonServices
{
    public class RecipeService : IRecipeService
    {
        private readonly IConverterService _converterService;
        private readonly IApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IElasticSearchService _elasticsearchService;
        public RecipeService(IConverterService converterService, IApplicationDbContext context, IMapper mapper, IElasticSearchService elasticSearchService)
        {
            _converterService = converterService ?? throw new ArgumentNullException(nameof(converterService));
            _context = context;
            _mapper = mapper;
            _elasticsearchService = elasticSearchService;
        }
        public List<RecipeDto> FilterDiets(List<int> diets, List<RecipeDto> recipes)
        {
            if (diets.Any())
            {
                recipes = recipes.Where(x => x.Ingredients.Select(x => x.Ingredient.DietTypes).All(eid => !diets.Except(eid.Select(ing => ing.Id)).Any())).ToList();
            }

            return recipes;
        }

        public async Task<List<RecipeDto>> FilterUsingRecipeNameAsync(string recipeName, List<RecipeDto> recipes)
        {
            if (!string.IsNullOrEmpty(recipeName))
            {
                var elasticModel = await _elasticsearchService.SearchByRecipeNameAsync(recipeName);

                recipes = recipes.Where(x => elasticModel.Any(e => e.Id == x.Id)).ToList();
            }

            return recipes;
        }

        public List<RecipeDto> FilterUsingRecipeByName(string recipeBy, List<RecipeDto> recipes)
        {
            if (!string.IsNullOrEmpty(recipeBy))
            {
                var matches = recipes
                    .Select(recipe => new { Recipe = recipe, Score = recipe.Credit.ToLower().JaroWinklerDistance(recipeBy.ToLower()) })
                    .OrderByDescending(match => match.Score)
                    .Where(match => match.Score >= 0.5)
                    .Take(10)
                    .Select(x => x.Recipe)
                    .ToList();

                recipes = matches;
            }

            return recipes;
        }


        public IQueryable<Recipe> ExcludeRecipes(List<int> recipesToExclude, IQueryable<Recipe> recipesEntites)
        {
            foreach (var recipeId in recipesToExclude)
            {
                recipesEntites = recipesEntites.Where(x => x.Id != recipeId);
            }

            return recipesEntites;
        }

        public List<RecipeDto> FilterDishTypes(List<int> dishTypes, List<RecipeDto> recipes)
        {
            if (dishTypes.Any()) 
            {
                recipes = recipes.Where(x => x.DishTypes.Any(dt => dishTypes.Contains(dt.Id))).ToList();
            }
            return recipes;
        }

        public List<RecipeDto> FilterCuisineTypes(List<int> cuisineTypes, List<RecipeDto> recipes)
        {
            if (cuisineTypes.Any()) 
            {
                recipes = recipes.Where(x => x.CuisineTypes.Any(ct => cuisineTypes.Contains(ct.Id))).ToList();
            }
            return recipes;
        }

        public List<RecipeDto> FilterIngredients(List<int> ingredients, List<RecipeDto> recipes)
        {
            if (ingredients.Any()) 
            {
                recipes = recipes.Where(x => x.Ingredients.Any(ing => ingredients.Contains(ing.Ingredient.Id))).ToList();
            }
            return recipes;
        }
      

        public List<RecipeDto> OrderRecipesByIngredients(List<int> ingredientIds, List<RecipeDto> recipes) 
        {
            foreach (var recipe in recipes)
            {
                var count = 0;
                foreach (var ingredient in recipe.Ingredients)
                {
                    if (ingredientIds.Contains(ingredient.Ingredient.Id))
                    {
                        ingredient.UserHasIngredient = true;
                        count++;
                    }
                }
                recipe.NumberOfUsedIngredients = count;
            }

            return recipes.OrderByDescending(x => (x.NumberOfUsedIngredients * 100) / x.NumberOfIngredients).ToList();
        }

        public RecipeDto GetRecipeInServingSize(int servingSize, RecipeDto recipe) 
        {
            // The ingredient converter will be used to get one serving size of current recipe
            decimal ingredientConverter = decimal.Divide(1, recipe.Servings);
            recipe.Servings = servingSize;
            foreach (var ingredient in recipe.Ingredients)
            {
                // Get ingredient amount for one serving size, then use that to get for serving size requested
                var ingredientAmountForOneServing = ingredient.Amount * ingredientConverter;
                var ingredientAmountForRequestingServing = ingredientAmountForOneServing * servingSize;

                // if the amount falls below 1 then we convert to grams for better experience
                if (ingredientAmountForRequestingServing < 1 && ingredient.Measurement.Name != "Grams") 
                {
                    var measurementType = _context.IngredientMeasurements.FirstOrDefault(x => x.MeasurementId == ingredient.Measurement.Id &&
                                            x.IngredientId == ingredient.Ingredient.Id);
                    var ingredientGrams = measurementType.AverageGrams != null ? measurementType.AverageGrams.Value * ingredient.Amount : Convert.ToDecimal(ingredient.Amount);
                    ingredient.Amount = (int)ingredientGrams;
                    ingredient.Measurement = _mapper.Map<MeasurementTypeDto>(_context.MeasurementTypes.FirstOrDefault(x => x.Name == "Grams"));
                } else 
                {
                    ingredient.Amount = (int)ingredientAmountForRequestingServing;
                }
                // we need to first convert the base nutrition for the amount based on their measurement
                // so lets say we have 10 tbs of olive oil, but the standard is 800g worth, the calories are way off, so we need to get for 
                // the default servings, how much calories. Once we have that we can get it for one serving size and times it by the new serving sizes.
                // which is the code we have here already. So before we come here we need to ensure we have the correct calories for the default serving size.
                // 
                // we convert each nutrition amount to the serving size amount
                ingredient.Ingredient.Calories = (int)(ingredient.Ingredient.Calories * ingredientConverter * servingSize);
                ingredient.Ingredient.Protein = ingredient.Ingredient.Protein * ingredientConverter * servingSize;
                ingredient.Ingredient.Carb = ingredient.Ingredient.Carb * ingredientConverter * servingSize;
                ingredient.Ingredient.Fat = ingredient.Ingredient.Fat * ingredientConverter * servingSize;
                ingredient.Ingredient.Sugar = ingredient.Ingredient.Sugar * ingredientConverter * servingSize;
                ingredient.Ingredient.Fibre = ingredient.Ingredient.Fibre * ingredientConverter * servingSize;
            
            }
            return recipe;
        }

         public RecipeDto GetRecipeInServingSizeWithoutNutrition(int servingSize, RecipeDto recipe) 
        {
            // The ingredient converter will be used to get one serving size of current recipe
            decimal ingredientConverter = decimal.Divide(1, recipe.Servings);
            recipe.Servings = servingSize;
            foreach (var ingredient in recipe.Ingredients)
            {
                // Get ingredient amount for one serving size, then use that to get for serving size requested
                var ingredientAmountForOneServing = ingredient.Amount * ingredientConverter;
                var ingredientAmountForRequestingServing = ingredientAmountForOneServing * servingSize;

                // if the amount falls below 1 then we convert to grams for better experience
                if (ingredientAmountForRequestingServing < 1 && ingredient.Measurement.Name != "Grams") 
                {
                    var measurementType = _context.IngredientMeasurements.FirstOrDefault(x => x.MeasurementId == ingredient.Measurement.Id &&
                                            x.IngredientId == ingredient.Ingredient.Id);
                    var ingredientGrams = measurementType.AverageGrams != null ? measurementType.AverageGrams.Value * ingredient.Amount : Convert.ToDecimal(ingredient.Amount);
                    ingredient.Amount = (int)ingredientGrams;
                    ingredient.Measurement = _mapper.Map<MeasurementTypeDto>(_context.MeasurementTypes.FirstOrDefault(x => x.Name == "Grams"));
                } else 
                {
                    ingredient.Amount = (int)ingredientAmountForRequestingServing;
                }
            }
            return recipe;
        }


        public List<RecipeDto> PopulateUserFavourites(List<UserFavouriteRecipes> favourites, List<RecipeDto> recipes)
        {
            recipes.ForEach(recipe => recipe.isFavourited = favourites.Any(x => x.RecipeId == recipe.Id));
            return recipes;
        }

        public RecipeDto PopulateUserFavourites(List<UserFavouriteRecipes> favourites, RecipeDto recipe)
        {
            recipe.isFavourited = favourites.Any(x => x.RecipeId == recipe.Id);
            return recipe;
        }

        public List<RecipeDto> CalculateNutrition(List<RecipeDto> recipes)
        {
            foreach (var recipe in recipes)
            {
                // foreach ingredient in the recipe we convert the nutritional values are relevant to the serving size of the recipe
                foreach (var ingredient in recipe.Ingredients)
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
                var recipeInServingSize = GetRecipeInServingSize(recipe.Servings, recipe);

                recipeInServingSize.Nutrition = new NutritionDto()
                {
                    Calories = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Calories),
                    Protein = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Protein),
                    Fat = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fat),
                    Fibre = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fibre),
                    Carb = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Carb),
                    Sugar = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Sugar)
                };
            }
            return recipes;
        }

        public RecipeDto CalculateNutrition(int servingSize, RecipeDto recipe)
        {
            var recipeInServingSize = GetRecipeInServingSize(servingSize, recipe);

            
            recipeInServingSize.Nutrition = new NutritionDto()
            {
                Calories = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Calories) / servingSize,
                Protein = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Protein) / servingSize,
                Fat = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fat) / servingSize,
                Fibre = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Fibre) / servingSize,
                Carb = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Carb) / servingSize,
                Sugar = recipeInServingSize.Ingredients.Sum(x => x.Ingredient.Sugar) / servingSize
            };
            return recipe;
        }
    }
}
