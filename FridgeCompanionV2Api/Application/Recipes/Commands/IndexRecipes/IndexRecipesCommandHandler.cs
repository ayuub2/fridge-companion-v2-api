
using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Recipes.Commands.IndexRecipes;
using FridgeCompanionV2Api.Application.Stats.Queries.GetStats;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Commands
{
    public class IndexRecipesCommandHandler : IRequestHandler<IndexRecipesCommand, Unit>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IElasticSearchService _elasticsearchService;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public IndexRecipesCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<IndexRecipesCommandHandler> logger, IElasticSearchService elasticSearchService)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _elasticsearchService = elasticSearchService ?? throw new ArgumentNullException(nameof(_elasticsearchService));
        }

        public async Task<Unit> Handle(IndexRecipesCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var recipes = _applicationDbContext.Recipes.Where(x => !x.IsDeleted).ToList();
            await _elasticsearchService.BulkIndexRecipesAsync(recipes);

            return await Task.FromResult(Unit.Value);
        }
    }
}
