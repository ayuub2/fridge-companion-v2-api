using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.CuisineTypes.Queries.GetCuisineTypes
{
    public class GetIngredientGroupTypesQueryHandler : IRequestHandler<GetIngredientGroupTypesQuery, List<IngredientGroupTypeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetIngredientGroupTypesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetIngredientGroupTypesQuery> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<IngredientGroupTypeDto>> Handle(GetIngredientGroupTypesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation($"Getting ingredient group types for user {request.UserId}.");

            var types = _applicationDbContext.IngredientGroupTypes.ToList();

            return _mapper.Map<List<IngredientGroupTypeDto>>(types);
        }
    }
}
