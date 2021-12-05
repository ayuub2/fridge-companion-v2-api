using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
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
                recipes = recipes.Where(x => x.Ingredients.Any(ing => ingredients.Contains(ing.Id))).ToList();
            }
            return recipes;
        }

        public List<RecipeDto> FilterUsingFridgeItems(List<FridgeItem> fridgeItems, List<RecipeDto> recipes)
        {
            if (fridgeItems.Any())
            {
                var fridgeItemIngredientIds = fridgeItems.Select(x => x.IngredientId).ToList();
                recipes = recipes.Where(x => x.Ingredients.Any(ing => fridgeItemIngredientIds.Contains(ing.Id))).ToList();
            }
            return recipes;
        }
    }
}
