using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Commands.AddIngredientToFridge
{
    public class AddIngredientToFridgeCommandValidator : AbstractValidator<AddIngredientToFridgeCommand>
    {
        public AddIngredientToFridgeCommandValidator()
        {
            RuleFor(x => x.Amount).NotEmpty().WithMessage("Amount must be more than 0".);
            RuleFor(x => x.ExpirationDate).NotEmpty().WithMessage("Please supply a valid datetime.");
            RuleFor(x => x.IngredientId).NotEmpty().WithMessage("Please supply a valid ingredientId.");
            RuleFor(x => x.MeasurementId).NotEmpty().WithMessage("Please supply a valid measurementId.");
            RuleFor(x => x.LocationId).NotEmpty().WithMessage("Please supply a valid locationId.");
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
        }
    }
}
