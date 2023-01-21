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
    public class GetCuisineTypesQueryHandler : IRequestHandler<GetCuisineTypesQuery, List<CuisineTypeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetCuisineTypesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetCuisineTypesQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<CuisineTypeDto>> Handle(GetCuisineTypesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation($"Getting cuisine types for user {request.UserId}.");

            var types = _applicationDbContext.CuisineTypes.ToList();

            return _mapper.Map<List<CuisineTypeDto>>(types);
        }
    }
}
