using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IRecipeService
    {
        List<RecipeDto> FilterDiets(List<int> diets, List<RecipeDto> recipes);
        List<RecipeDto> FilterUsingRecipeName(string recipeName, List<RecipeDto> recipes);
        IQueryable<Recipe> ExcludeRecipes(List<int> recipesToExclude, IQueryable<Recipe> recipesEntites);
        List<RecipeDto> FilterDishTypes(List<int> dishTypes, List<RecipeDto> recipes);
        List<RecipeDto> FilterCuisineTypes(List<int> cuisineTypes, List<RecipeDto> recipes);
        List<RecipeDto> FilterIngredients(List<int> ingredients, List<RecipeDto> recipes);
        List<RecipeDto> FilterUsingFridgeItems(List<FridgeItem> fridgeItems, List<RecipeDto> recipes);
    }
}