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

        public GetFilteredRecipesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetFilteredRecipesQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

            // TODO: filter out excluded recipes
            foreach (var recipeId in request.RecipesToExclude)
            {
                recipesEntites = recipesEntites.Where(x => x.Id != recipeId);
            }

            // Convert to DTO
            var recipes = _mapper.Map<List<RecipeDto>>(recipesEntites.ToList());

            // TODO: filter using recipe name
            if (!string.IsNullOrEmpty(request.RecipeName))
            {
                var results = Process.ExtractAll(new RecipeDto() { Name = request.RecipeName }, recipes, recipe => recipe.Name, cutoff: 60);
                recipes = results.Select(x => x.Value).ToList();
            }

            return recipes;

            // TODO: filter using ingredient groups
            // TODO: filter using diets
            // TODO: filter using dishtypes
            // TODO: filter using cuisine types
            // TODO: filter using ingredients
            // TODO: filter isNutFree
            // TODO: filter isGlutenFree
            // TODO: filter proteins
            // TODO: filter calories
            // TODO: filter sugar
            // TODO: filter fat
            // TODO: filter carbs

        }
    }
}
