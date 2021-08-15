using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzySharp;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetIngredientsByName
{
    public class GetIngredientByNameQueryHandler : IRequestHandler<GetIngredientByNameQuery, List<IngredientDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetIngredientByNameQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetIngredientByNameQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<IngredientDto>> Handle(GetIngredientByNameQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var ingredients = _applicationDbContext.Ingredients.Where(x => !x.IsDeleted).ToList();
            var foundIngredients = new List<Ingredient>();

            foreach (var ingredientName in request.IngredientNames)
            {
                var result = Process.ExtractOne(ingredientName, ingredients.Select(x => x.Name).ToArray(), cutoff: 70);
                if (result is not null) 
                {
                    foundIngredients.Add(ingredients.ElementAt(result.Index));
                }
            }

            return _mapper.Map<List<IngredientDto>>(foundIngredients);
        }
    }
}
