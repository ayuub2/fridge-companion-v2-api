using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class CreateShoppingItemCommandValidator : AbstractValidator<CreateShoppingItemCommand>
    {
        public CreateShoppingItemCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Item name cannot be empty.");
        }
    }
}
