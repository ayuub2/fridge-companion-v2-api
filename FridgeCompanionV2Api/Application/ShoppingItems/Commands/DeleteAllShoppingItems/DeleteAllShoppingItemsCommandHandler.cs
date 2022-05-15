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

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteAllShoppingItems
{
    public class DeleteAllShoppingItemsCommandHandler : IRequestHandler<DeleteAllShoppingItemsCommand, List<ShoppingItemDto>>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteAllShoppingItemsCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<DeleteAllShoppingItemsCommand> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<ShoppingItemDto>> Handle(DeleteAllShoppingItemsCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var shoppingList = _applicationDbContext.ShoppingLists.FirstOrDefault(x => x.UserId == request.UserId);
            if (shoppingList is null) 
            {
                _logger.LogError($"Unable to delete all shopping items for user - {request.UserId}");
                throw new NotFoundException("No shopping list for user, please add an item first.");
            }
            var items = _applicationDbContext.ShoppingListItems.Where(x => x.ShoppingListId == shoppingList.Id).ToList();
            if (!items.Any())
            {
                _logger.LogError($"Unable to delete all shopping items for user - {request.UserId}");
                throw new NotFoundException("Shopping Items not found");
            }

            items.ForEach(x => x.IsDeleted = true);
            
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<List<ShoppingItemDto>>(items);
        }

        
    }
}
