using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeById
{
    public class GetRecipeByIdQueryHandler : IRequestHandler<GetRecipeByIdQuery, RecipeDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly IRecipeService _recipeService;

        public GetRecipeByIdQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetRecipeByIdQueryHandler> logger, IRecipeService recipeService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _recipeService = recipeService ?? throw new ArgumentNullException(nameof(recipeService));
        }

        public async Task<RecipeDto> Handle(GetRecipeByIdQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var recipe = _applicationDbContext.GetRecipesWithDetails().FirstOrDefault(x => x.Id == request.RecipeId);
            var recipeDto = _mapper.Map<RecipeDto>(recipe);
            if (recipe is null) throw new NotFoundException("Recipe not found.");
            return _recipeService.OrderRecipesByIngredients(_applicationDbContext.FreshFridgeItems(request.UserId).Select(x => x.IngredientId).ToList(), new List<RecipeDto>() { recipeDto }).FirstOrDefault();
        }
    }
}
