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

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class CreateShoppingItemCommandHandler : IRequestHandler<CreateShoppingItemCommand, ShoppingItemDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateShoppingItemCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<CreateShoppingItemCommand> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingItemDto> Handle(CreateShoppingItemCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            try
            {
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

                var addedItem = _applicationDbContext.ShoppingListItem.Add(new Domain.Entities.ShoppingListItem() 
                {
                    //Name = request.Name,
                    IsChecked = false,
                    IsDeleted = false,
                    ShoppingList = shoppingList,
                    ShoppingListId = shoppingList.Id
                });

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<ShoppingItemDto>(addedItem.Entity);
            }
            catch (Exception exc)
            {
                _logger.LogError($"Unable to add new shopping list item for user - {request.UserId}", exc);
                throw exc;
            }

        }
    }
}
