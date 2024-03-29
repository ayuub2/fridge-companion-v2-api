﻿using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzySharp;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes
{
    public class GetFilteredRecipesQueryHandler : IRequestHandler<GetFilteredRecipesQuery, List<RecipeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRecipeService _recipeService;
        private readonly IConverterService _converterService;

        public GetFilteredRecipesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetFilteredRecipesQueryHandler> logger, IRecipeService recipeService, IConverterService converterService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
            _converterService = converterService ?? throw new ArgumentNullException(nameof(converterService));
        }
        public async Task<List<RecipeDto>> Handle(GetFilteredRecipesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipesEntites = _applicationDbContext.GetRecipesWithDetails();

            var user = _applicationDbContext.Users
                .Include(x => x.UserDiets)
                    .ThenInclude(x => x.DietType)
                .Include(x => x.UserFavouriteRecipes)
                .FirstOrDefault(x => x.Id == request.UserId);

            // Filter out excluded recipes
            recipesEntites = _recipeService.ExcludeRecipes(request.RecipesToExclude, recipesEntites);

            // Convert to DTO
            var recipes = _mapper.Map<List<RecipeDto>>(recipesEntites.ToList());

            // Filter using recipe name
            recipes = await _recipeService.FilterUsingRecipeNameAsync(request.RecipeName, recipes);

            // Filter using diets
            recipes = _recipeService.FilterDiets(request.Diets, recipes);

            // Filter using dish types
            recipes = _recipeService.FilterDishTypes(request.DishTypes, recipes);

            // Filter using cuisine types
            recipes = _recipeService.FilterCuisineTypes(request.CuisineTypes, recipes);

            // Filter using ingredients
            recipes = _recipeService.FilterIngredients(request.Ingredients, recipes);

            // Filter using protein
            if (request.Protein is not null) 
            {
                recipes = FilterNutrition(recipes, request.Protein, Nutrition.Protein);
            }

            // Filter using calories
            if (request.Calories is not null)
            {
                recipes = FilterNutrition(recipes, request.Calories, Nutrition.Calories);
            }

            // Filter using sugar
            if (request.Sugar is not null)
            {
                recipes = FilterNutrition(recipes, request.Sugar, Nutrition.Sugar);
            }

            // Filter using fat
            if (request.Fat is not null)
            {
                recipes = FilterNutrition(recipes, request.Fat, Nutrition.Fat);
            }

            // Filter using carbs
            if (request.Carbs is not null)
            {
                recipes = FilterNutrition(recipes, request.Carbs, Nutrition.Carb);
            }

            if(request.recipeUnderMinutes != 0)
            {
                recipes = recipes.Where(x => x.ReadyInMinutes <= request.recipeUnderMinutes).ToList();
            }

            if(!string.IsNullOrWhiteSpace(request.recipeBy))
            {
                recipes = _recipeService.FilterUsingRecipeByName(request.recipeBy, recipes);
            }

            // Fill out missing ingredients and sort
            if (request.UseUserIngredients)
            {
                var freshFridgeItems = _applicationDbContext.FreshFridgeItems(request.UserId).ToList();
                recipes = _recipeService.OrderRecipesByIngredients(freshFridgeItems.Select(x => x.IngredientId).ToList(), recipes);
            }
            if (request.Ingredients.Any() && !request.UseUserIngredients) 
            {
                recipes = _recipeService.OrderRecipesByIngredients(request.Ingredients, recipes);
            }

            if (!request.Ingredients.Any() && !request.UseUserIngredients) 
            {
                var freshFridgeItems = _applicationDbContext.FreshFridgeItems(request.UserId).ToList();
                recipes = _recipeService.OrderRecipesByIngredients(freshFridgeItems.Select(x => x.IngredientId).ToList(), recipes);
            }

            // Check if user favourited recipes are returned
            recipes = _recipeService.PopulateUserFavourites(user.UserFavouriteRecipes, recipes);

            return recipes.Take(30).ToList();
        }

        public  List<RecipeDto> FilterNutrition(List<RecipeDto> recipes, NutritionFilter nutritionFilter, Nutrition nutrition)
        {
            // To work out the nutrition we need to get each recipes ingredients.
            return recipes.Where(recipe =>
            {
                decimal recipeNutrition = 0;
                foreach (var ingredient in recipe.Ingredients)
                {

                    //For each ingredient we must get the measurement it was saved at and convert to grams
                    // Using the average grams of the measurement and the amount of that measurement we have, we can get the grams of that ingredient
                    var ingredientGrams = _converterService.ConvertIngredientAmountToGrams(ingredient);
                    // Since the nutrional information is saved at a 100 gram standard we need to divide the ingredient grams by the standard
                    var gramsFactor = ingredientGrams / ingredient.Ingredient.Standard;
                    // Finally we take the factor multiplied by the nutrition to give us the nutrition for the amount of grams we have
                    switch (nutrition)
                    {
                        case Nutrition.Protein:
                            recipeNutrition += (gramsFactor * ingredient.Ingredient.Protein);
                            break;
                        case Nutrition.Calories:
                            recipeNutrition += (gramsFactor * ingredient.Ingredient.Calories);
                            break;
                        case Nutrition.Sugar:
                            recipeNutrition += (gramsFactor * ingredient.Ingredient.Sugar);
                            break;
                        case Nutrition.Fat:
                            recipeNutrition += (gramsFactor * ingredient.Ingredient.Fat);
                            break;
                        case Nutrition.Carb:
                            recipeNutrition += (gramsFactor * ingredient.Ingredient.Carb);
                            break;
                        default:
                            break;
                    }
                }
                switch (nutritionFilter.Operator.ToUpper())
                {
                    case "GREATERTHAN":
                        return recipeNutrition > nutritionFilter.Amount;
                    case "LESSTHAN":
                        return recipeNutrition < nutritionFilter.Amount;
                    case "EQUAL":
                        return recipeNutrition == nutritionFilter.Amount;
                    default:
                        throw new Exception("Invalid Operator");
                }
            }).ToList();
        }
    }
}
