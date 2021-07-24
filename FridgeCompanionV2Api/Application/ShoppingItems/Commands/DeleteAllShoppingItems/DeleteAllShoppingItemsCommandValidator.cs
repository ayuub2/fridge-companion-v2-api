using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.CreateShoppingItem
{
    public class DeleteAllShoppingItemsCommandValidator : AbstractValidator<DeleteAllShoppingItemsCommand>
    {
        public DeleteAllShoppingItemsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
        }
    }
}
