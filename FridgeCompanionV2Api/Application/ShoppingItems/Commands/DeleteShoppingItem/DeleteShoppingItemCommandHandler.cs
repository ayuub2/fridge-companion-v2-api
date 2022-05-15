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

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteShoppingItem
{
    public class DeleteShoppingItemCommandHandler : IRequestHandler<DeleteShoppingItemCommand, ShoppingItemDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public DeleteShoppingItemCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<DeleteShoppingItemCommand> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingItemDto> Handle(DeleteShoppingItemCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var item = _applicationDbContext.ShoppingListItems.FirstOrDefault(x => x.Id == request.Id);
            if (item is null) 
            {
                _logger.LogError($"Unable to delete shopping item for user - {request.UserId}");
                throw new NotFoundException("Shopping Item not found");
            }


            item.IsDeleted = true;

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ShoppingItemDto>(item);
        }
    }
}
