﻿using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Queries.GetAllFridgeItems
{
    public class GetAllFridgeItemsQueryHandler : IRequestHandler<GetAllFridgeItemsQuery, List<FridgeItemDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetAllFridgeItemsQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetAllFridgeItemsQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<FridgeItemDto>> Handle(GetAllFridgeItemsQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var items = _applicationDbContext.FridgeItems.Where(x => x.UserId == request.UserId && !x.IsDeleted).ToList();
            return _mapper.Map<List<FridgeItemDto>>(items);
        }
    }
}