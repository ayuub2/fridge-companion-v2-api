using FluentValidation;
using FridgeCompanionV2Api.Application.Common.Interfaces;
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
        public CreateShoppingItemCommandValidator(IApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;

            RuleFor(x => x.MeasurementId)
                .NotEmpty().WithMessage("Item must have a measurement.")
                .MustAsync(ExistMeasurementType).WithMessage("Measurement type does not exist.");
            RuleFor(x => x.IngredientId)
                .MustAsync(ExistIngredientType).WithMessage("Ingredient does not exist.");
        }

        private async Task<bool> ExistMeasurementType(int measurementId, CancellationToken cancellationToken)
        {
            var measurementType = await _applicationDbContext.MeasurementTypes.FindAsync(measurementId);
            return measurementType is not null;
        }

        private async Task<bool> ExistIngredientType(int ingredientId, CancellationToken cancellationToken)
        {
            var ingredient = await _applicationDbContext.Ingredients.FindAsync(ingredientId);
            return ingredient is not null;
        }
    }
}
