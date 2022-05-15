using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteShoppingItem
{
    public class DeleteShoppingItemCommandValidator : AbstractValidator<DeleteShoppingItemCommand>
    {
        public DeleteShoppingItemCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Item Id cannot be empty.");
        }
    }
}
