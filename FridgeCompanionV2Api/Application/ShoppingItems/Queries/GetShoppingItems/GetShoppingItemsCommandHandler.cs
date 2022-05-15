using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Exceptions;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Queries.GetShoppingItems
{
    public class GetShoppingItemsCommandHandler : IRequestHandler<GetShoppingItemsCommand, List<ShoppingItemDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public GetShoppingItemsCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<GetShoppingItemsCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ShoppingItemDto>> Handle(GetShoppingItemsCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var shoppingList = _applicationDbContext.ShoppingLists.FirstOrDefault(x => x.UserId == request.UserId);
            if (shoppingList is null)
            {
                _logger.LogError($"Unable to  get shopping items for user - {request.UserId}");
                throw new NotFoundException("No shopping list for user, please add an item first.");
            }
            var items = _applicationDbContext.ShoppingListItems.Where(x => x.ShoppingListId == shoppingList.Id && !x.IsDeleted).ToList();
            if (!items.Any())
            {
                _logger.LogError($"Unable to get all shopping items for user - {request.UserId}");
                throw new NotFoundException("Shopping Items not found");
            }
            return _mapper.Map<List<ShoppingItemDto>>(items);
        }
    }
}
