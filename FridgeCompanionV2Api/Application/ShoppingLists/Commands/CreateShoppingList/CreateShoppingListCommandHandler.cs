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

namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList
{
    public class CreateShoppingListCommandHandler : IRequestHandler<CreateShoppingListCommand, ShoppingListDto>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CreateShoppingListCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ShoppingListDto> Handle(CreateShoppingListCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }


            try
            {
                var addedEntity = _applicationDbContext.ShoppingLists.Add(new Domain.Entities.ShoppingList()
                {
                    Name = request.Name,
                    UserId = request.UserId
                });

                await _applicationDbContext.SaveChangesAsync(cancellationToken);
                return _mapper.Map<ShoppingListDto>(addedEntity);
            }
            catch (Exception exc) 
            {
                _logger.LogInformation($"Unable to add new shopping list for user - {request.UserId}", exc);
                throw exc;
            }
        }
    }
}
