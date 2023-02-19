using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.CuisineTypes.Queries.GetCuisineTypes
{
    public class GetSuggestionsQueryHandler : IRequestHandler<GetSuggestionsQuery, List<SuggestionDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetSuggestionsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetSuggestionsQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<SuggestionDto>> Handle(GetSuggestionsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation($"Getting suggestions for user {request.UserId}.");

            var suggestions = _applicationDbContext.Suggestions.Where(x => !x.IsDeleted).Include(x => x.Recipes).ThenInclude(x => x.Recipe).ToList();

            var suggestionsDto = suggestions.Select(x => new SuggestionDto()
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.ImageUrl,
                Recipes = _mapper.Map<List<RecipeDto>>(x.Recipes.Select(x => x.Recipe).ToList())
            });

            return suggestionsDto.ToList();
        }
    }
}
