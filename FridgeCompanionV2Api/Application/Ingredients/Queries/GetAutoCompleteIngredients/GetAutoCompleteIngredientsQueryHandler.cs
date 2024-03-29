﻿using AutoMapper;
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
        private readonly IElasticSearchService _elasticsearchService;

        public GetAutoCompleteIngredientsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetAutoCompleteIngredientsQueryHandler> logger, IElasticSearchService elasticSearchService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticsearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(_elasticsearchService));
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
                .Where(x => !x.IsDeleted);

            var elasticModels = await _elasticsearchService.SearchByIngredientNameAsync(request.Query);

            var matches = new List<Ingredient>();
            foreach (var elasticModel in elasticModels)
            {
                matches.Add(ingredients.FirstOrDefault(x => x.Id  == elasticModel.Id));
            }
            return _mapper.Map<List<IngredientDto>>(matches);
        }
    }
}
