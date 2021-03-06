using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzySharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.CommonServices
{
    public class RecipeService : IRecipeService
    {
        public List<RecipeDto> FilterDiets(List<int> diets, List<RecipeDto> recipes)
        {
            if (diets.Any())
            {
                recipes = recipes.Where(x => x.Ingredients.Select(x => x.Ingredient.DietTypes).All(eid => !diets.Except(eid.Select(ing => ing.Id)).Any())).ToList();
            }

            return recipes;
        }

        public List<RecipeDto> FilterUsingRecipeName(string recipeName, List<RecipeDto> recipes)
        {
            if (!string.IsNullOrEmpty(recipeName))
            {
                var results = Process.ExtractAll(new RecipeDto() { Name = recipeName }, recipes, recipe => recipe.Name, cutoff: 60);
                recipes = results.Select(x => x.Value).ToList();
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

        public List<RecipeDto> FilterUsingFridgeItems(List<FridgeItem> fridgeItems, List<RecipeDto> recipes)
        {
            if (fridgeItems.Any())
            {
                var fridgeItemIngredientIds = fridgeItems.Select(x => x.IngredientId).ToList();
                recipes = recipes.Where(x => x.Ingredients.Any(ing => fridgeItemIngredientIds.Contains(ing.Ingredient.Id))).ToList();

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
                        count++;
                }
                recipe.NumberOfUsedIngredients = count;
            }

            return recipes.OrderByDescending(x => (x.NumberOfUsedIngredients * 100) / x.NumberOfIngredients).ToList();
        }

        public List<RecipeDto> FilterGlutenRecipes(List<RecipeDto> recipes) 
        {
            return recipes.Where(x => x.CuisineTypes.Any(x => x.Name == "Gluten Free")).ToList();
        }

        public RecipeDto GetRecipeInServingSize(int ServingSize, RecipeDto recipe, IApplicationDbContext dbcontext, IMapper mapper) 
        {
            // The ingredient converter will be used to get one serving size of current recipe
            decimal ingredientConverter = decimal.Divide(1, recipe.Servings);
            recipe.Servings = ServingSize;
            foreach (var ingredient in recipe.Ingredients)
            {
                // Get ingredient amount for one serving size, then use that to get for serving size requested
                var ingredientAmountForOneServing = ingredient.Amount * ingredientConverter;
                var ingredientAmountForRequestingServing = ingredientAmountForOneServing * ServingSize;

                // if the amount falls below 1 then we convert to grams for better experience
                if (ingredientAmountForRequestingServing < 1 && ingredient.Measurement.Name != "Grams") 
                {
                    var measurementType = dbcontext.IngredientMeasurements.FirstOrDefault(x => x.MeasurementId == ingredient.Measurement.Id &&
                                            x.IngredientId == ingredient.Ingredient.Id);
                    var ingredientGrams = measurementType.AverageGrams != null ? measurementType.AverageGrams.Value * ingredient.Amount : Convert.ToDecimal(ingredient.Amount);
                    ingredient.Amount = (int)ingredientGrams;
                    ingredient.Measurement = mapper.Map<MeasurementTypeDto>(dbcontext.MeasurementTypes.FirstOrDefault(x => x.Name == "Grams"));
                } else 
                {
                    ingredient.Amount = (int)ingredientAmountForRequestingServing;
                }

                // we convert each nutrition amount to the serving size amount
                ingredient.Ingredient.Calories = (int)(ingredient.Ingredient.Calories * ingredientConverter * ServingSize);
                ingredient.Ingredient.Protein = ingredient.Ingredient.Protein * ingredientConverter * ServingSize;
                ingredient.Ingredient.Carb = ingredient.Ingredient.Carb * ingredientConverter * ServingSize;
                ingredient.Ingredient.Fat = ingredient.Ingredient.Fat * ingredientConverter * ServingSize;
                ingredient.Ingredient.Sugar = ingredient.Ingredient.Sugar * ingredientConverter * ServingSize;
                ingredient.Ingredient.Fibre = ingredient.Ingredient.Fibre * ingredientConverter * ServingSize;
            
            }
            return recipe;
        }

        public List<RecipeDto> RemoveRecipesContainingNuts(List<RecipeDto> recipes)
        {
            throw new NotImplementedException();
        }
    }
}
