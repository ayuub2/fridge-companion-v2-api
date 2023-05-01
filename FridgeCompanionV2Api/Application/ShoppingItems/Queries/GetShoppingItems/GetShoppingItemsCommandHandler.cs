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
            var shoppingList = _applicationDbContext.ShoppingList.FirstOrDefault(x => x.UserId == request.UserId);
            if (shoppingList is null)
            {
                shoppingList = _applicationDbContext.ShoppingList.Add(new Domain.Entities.ShoppingList()
                {
                    Name = "Default shopping list",
                    UserId = request.UserId
                }).Entity;
                await _applicationDbContext.SaveChangesAsync(cancellationToken);
            }
            var items = _applicationDbContext.ShoppingListItem.Where(x => x.ShoppingListId == shoppingList.Id && !x.IsDeleted).ToList();
            
            return _mapper.Map<List<ShoppingItemDto>>(items);
        }
    }
}
