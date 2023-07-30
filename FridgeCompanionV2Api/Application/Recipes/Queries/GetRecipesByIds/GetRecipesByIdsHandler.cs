using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipes;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipesByIds
{
    public class GetRecipesByIdsHandler : IRequestHandler<GetRecipesByIdsQuery, List<RecipeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRecipeService _recipeService;

        public GetRecipesByIdsHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetRecipesByIdsQuery> logger, IRecipeService recipeService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        }

        public async Task<List<RecipeDto>> Handle(GetRecipesByIdsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            _logger.LogInformation("retrieving recipes for user id {UserId}", request.UserId);
            var recipes = _applicationDbContext.GetRecipesWithDetails().Where(x => request.RecipeIds.Contains(x.Id)).ToList();
            var recipeDtos = _mapper.Map<List<RecipeDto>>(recipes);
            
            return recipeDtos;
        }
    }
}
