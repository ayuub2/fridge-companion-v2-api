using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Queries.GetShoppingList
{
    public class GetShoppingListQueryHandler : IRequestHandler<GetShoppingListQuery, ShoppingListDto>
    {

        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetShoppingListQueryHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetShoppingListQueryHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingListDto> Handle(GetShoppingListQuery request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            _logger.LogInformation("Getting shopping list for user {UserId}", request.UserId);
            var shoppingList = _applicationDbContext.ShoppingList
                .Include(x => x.Recipes)
                    .ThenInclude(x => x.Recipe)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Measurement)
                .Include(x => x.Items)
                    .ThenInclude(x => x.Ingredient)
                .FirstOrDefault(x => x.UserId == request.UserId);

            if (shoppingList is null)
            {
                _logger.LogInformation("Creating default shopping list for user {UserId}", request.UserId);
                shoppingList = _applicationDbContext.ShoppingList.Add(new Domain.Entities.ShoppingList()
                {
                    Name = "Default shopping list",
                    UserId = request.UserId
                }).Entity;
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }

            return _mapper.Map<ShoppingListDto>(shoppingList);
        }
    }
}
