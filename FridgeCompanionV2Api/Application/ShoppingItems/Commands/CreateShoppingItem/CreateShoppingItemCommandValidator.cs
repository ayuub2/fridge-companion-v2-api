using FluentValidation;
using FridgeCompanionV2Api.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class CreateShoppingItemCommandValidator : AbstractValidator<CreateShoppingItemCommand>
    {
        private readonly IApplicationDbContext _applicationDbContext;
        private readonly ICurrentUserService _currentUserService;
        public CreateShoppingItemCommandValidator(IApplicationDbContext applicationDbContext, ICurrentUserService currentUserService)
        {
            _applicationDbContext = applicationDbContext;
            _currentUserService = currentUserService;

            RuleFor(x => x.MeasurementId)
                .NotEmpty().WithMessage("Item must have a measurement.")
                .MustAsync(MeasurementTypeExists).WithMessage("Measurement type does not exist.");
            RuleFor(x => x.IngredientId)
                .MustAsync(IngredientExists).WithMessage("Ingredient does not exist.")
                .MustAsync((command, properties, cancellationToken) => ItemNotExists(command.IngredientId, _currentUserService.UserId, cancellationToken)).WithMessage("Ingredient already exists in your shopping list, please update ingredient instead.");
        }

        private async Task<bool> MeasurementTypeExists(int measurementId, CancellationToken cancellationToken)
        {
            var measurementType = await _applicationDbContext.MeasurementTypes.FindAsync(measurementId);
            return measurementType is not null;
        }

        private async Task<bool> IngredientExists(int ingredientId, CancellationToken cancellationToken)
        {
            var ingredient = await _applicationDbContext.Ingredients.FindAsync(ingredientId);
            return ingredient is not null;
        }

        private async Task<bool> ItemNotExists(int ingredientId, string userId, CancellationToken cancellationToken)
        {
            var ingredient = await _applicationDbContext.ShoppingListItem.Include(x => x.ShoppingList).FirstOrDefaultAsync(x => x.IngredientId == ingredientId && x.ShoppingList.UserId == userId && x.IsDeleted != true);
            return ingredient is null;
        }
    }
}
