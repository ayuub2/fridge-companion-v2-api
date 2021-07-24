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
    public class UpdateShoppingItemCommandHandler : IRequestHandler<UpdateShoppingItemCommand, ShoppingItemDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public UpdateShoppingItemCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<UpdateShoppingItemCommand> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingItemDto> Handle(UpdateShoppingItemCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var item = _applicationDbContext.ShoppingListItems.FirstOrDefault(x => x.Id == request.Id);
            if (item is null) 
            {
                _logger.LogError($"Unable to update shopping item for user - {request.UserId}");
                throw new ArgumentException();
            }


            item.IsChecked = request.IsChecked;

            await _applicationDbContext.SaveChangesAsync(cancellationToken);
            return _mapper.Map<ShoppingItemDto>(item);
        }
    }
}
