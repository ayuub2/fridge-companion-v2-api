using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Domain.Entities;
using FuzzyString;
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
                .Include(x => x.MeasurementTypes)
                    .ThenInclude(idt => idt.Measurement)
                .Where(x => !x.IsDeleted).AsNoTracking().ToList();
           
            var matches = ingredients
            .Select(ingredient => new { Ingredient = ingredient, Score = ingredient.Name.ToLower().JaroWinklerDistance(request.Query.ToLower()) })
            .OrderByDescending(match => match.Score)
            .Where(match => match.Score >= 0.7)
            .Take(10)
            .Select(x => x.Ingredient)
            .ToList();

            return _mapper.Map<List<IngredientDto>>(matches);
        }
    }
}
