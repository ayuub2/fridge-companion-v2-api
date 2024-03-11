using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes;
using FridgeCompanionV2Api.Domain.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IRecipeService
    {
        List<RecipeDto> FilterDiets(List<int> diets, List<RecipeDto> recipes);
        Task<List<RecipeDto>> FilterUsingRecipeNameAsync(string recipeName, List<RecipeDto> recipes);
        IQueryable<Recipe> ExcludeRecipes(List<int> recipesToExclude, IQueryable<Recipe> recipesEntites);
        List<RecipeDto> FilterUsingRecipeByName(string recipeBy, List<RecipeDto> recipes);
        List<RecipeDto> FilterDishTypes(List<int> dishTypes, List<RecipeDto> recipes);
        List<RecipeDto> FilterCuisineTypes(List<int> cuisineTypes, List<RecipeDto> recipes);
        List<RecipeDto> FilterIngredients(List<int> ingredients, List<RecipeDto> recipes);
        List<RecipeDto> OrderRecipesByIngredients(List<int> ingredientIds, List<RecipeDto> recipes);
        RecipeDto CalculateNutrition(int servingSize, RecipeDto recipe);
        List<RecipeDto> PopulateUserFavourites(List<UserFavouriteRecipes> favourites, List<RecipeDto> recipes);
        RecipeDto PopulateUserFavourites(List<UserFavouriteRecipes> favourites, RecipeDto recipe);
        RecipeDto GetRecipeInServingSize(int servingSize, RecipeDto recipe);
        RecipeDto GetRecipeInServingSizeWithoutNutrition(int servingSize, RecipeDto recipe);
    }
}