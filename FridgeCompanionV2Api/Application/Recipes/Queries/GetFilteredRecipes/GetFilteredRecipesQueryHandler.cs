using AutoMapper;
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

        public GetFilteredRecipesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetFilteredRecipesQueryHandler> logger, IRecipeService recipeService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        }
        public async Task<List<RecipeDto>> Handle(GetFilteredRecipesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipesEntites = _applicationDbContext.Recipes
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
                    .ThenInclude(x => x.Cuisine)
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            // Filter out excluded recipes
            recipesEntites = _recipeService.ExcludeRecipes(request.RecipesToExclude, recipesEntites);

            // Convert to DTO
            var recipes = _mapper.Map<List<RecipeDto>>(recipesEntites.ToList());

            // Filter using recipe name
            recipes = _recipeService.FilterUsingRecipeName(request.RecipeName, recipes);

            // Filter using diets
            recipes = _recipeService.FilterDiets(request.Diets, recipes);

            // Filter using dish types
            recipes = _recipeService.FilterDishTypes(request.DishTypes, recipes);

            // Filter using cuisine types
            recipes = _recipeService.FilterCuisineTypes(request.CuisineTypes, recipes);

            // Filter using ingredients
            recipes = _recipeService.FilterIngredients(request.Ingredients, recipes);


            // Filter using users fridge items
            // TODO: Test this by adding items to fridge and seeing them comeback.
            if(request.UseUserIngredients) 
            {
                recipes = _recipeService.FilterUsingFridgeItems(_applicationDbContext.FreshFridgeItems(request.UserId).ToList(), recipes);
            }

            // TODO: filter isNutFree
            // TODO: filter isGlutenFree
            // TODO: filter proteins
            // TODO: filter calories
            // TODO: filter sugar
            // TODO: filter fat
            // TODO: filter carbs
            // TODO: calculate number of missing ingredients and sort based on that. Take top 10 and dont pad. Enure ingredients are calculated by either user provided ingredients
            // or items already in the fridge
            return recipes;
        }
    }
}
