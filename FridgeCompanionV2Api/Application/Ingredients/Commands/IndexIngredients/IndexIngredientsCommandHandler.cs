using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Recipes.Commands.IndexRecipes;
using FridgeCompanionV2Api.Application.Recipes.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Linq;

namespace FridgeCompanionV2Api.Application.Ingredients.Commands.IndexIngredients
{
    public class IndexIngredientsCommandHandler : IRequestHandler<IndexIngredientsCommand, Unit>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IElasticSearchService _elasticsearchService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public IndexIngredientsCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<IndexIngredientsCommandHandler> logger, IElasticSearchService elasticSearchService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticsearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(_elasticsearchService));
        }

        public async Task<Unit> Handle(IndexIngredientsCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var ingredients = _applicationDbContext.Ingredients.Where(x => !x.IsDeleted).ToList();
            await _elasticsearchService.BulkIndexIngredientsAsync(ingredients);

            return await Task.FromResult(Unit.Value);
        }
    }
}
