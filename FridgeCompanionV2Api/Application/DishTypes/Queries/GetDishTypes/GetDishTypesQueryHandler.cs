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
    public class GetDishTypesQueryHandler : IRequestHandler<GetDishTypesQuery, List<DishTypeDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetDishTypesQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetDishTypesQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<List<DishTypeDto>> Handle(GetDishTypesQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation($"Getting dish types for user {request.UserId}.");

            var types = _applicationDbContext.DishTypes.ToList();

            return _mapper.Map<List<DishTypeDto>>(types);
        }
    }
}
