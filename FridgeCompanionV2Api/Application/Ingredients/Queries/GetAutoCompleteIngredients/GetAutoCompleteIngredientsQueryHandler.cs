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

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetAutoCompleteIngredients
{
    public class GetAutoCompleteIngredientsQueryHandler : IRequestHandler<GetAutoCompleteIngredientsQuery, List<IngredientDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetAutoCompleteIngredientsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetAutoCompleteIngredientsQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<IngredientDto>> Handle(GetAutoCompleteIngredientsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var ingredients = _applicationDbContext.Ingredients
                .Include(x => x.DietTypes)
                    .ThenInclude(idt => idt.Diet)
                .Include(x => x.Location)
                .Include(x => x.GroupTypes)
                    .ThenInclude(idt => idt.IngredientGroupType)
                .Where(x => !x.IsDeleted).AsNoTracking().ToList();
            var results = Process.ExtractTop(request.Query, ingredients
                .Select(x => x.Name).ToArray(), limit: 10, cutoff: 60);
            var ingredientResults = new List<Ingredient>();
            results.ToList().ForEach(x =>
            {
                ingredientResults.Add(ingredients.ElementAt(x.Index));
            });

            return _mapper.Map<List<IngredientDto>>(ingredientResults);
        }
    }
}
