using AutoMapper;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Application.ShoppingRecipes.Commands.AddShoppingRecipe;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.ClearShoppingList
{
    public class ClearShoppingListCommandHandler : IRequestHandler<ClearShoppingListCommand, Unit>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public ClearShoppingListCommandHandler(IApplicationDbContext applicationDbContext, IMapper mapper, ILogger<ClearShoppingListCommandHandler> logger)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        public async Task<Unit> Handle(ClearShoppingListCommand request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            _logger.LogInformation("Clearing shopping list item for user {UserId}", request.UserId);

            var shoppingList = await _applicationDbContext.ShoppingList.Include(x => x.Items).Include(x => x.Recipes).FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (shoppingList == null) { throw new Exception("Create shopping list first by calling the get shopping list endpoint."); }

            foreach (var item in shoppingList.Recipes)
            {
                item.IsDeleted = true;
            }

            foreach (var item in shoppingList.Items)
            {
                item.IsDeleted = true;
            }

            await _applicationDbContext.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
