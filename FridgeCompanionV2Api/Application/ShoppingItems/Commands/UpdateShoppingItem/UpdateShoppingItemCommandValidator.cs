﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.UpdateShoppingItem
{
    public class UpdateShoppingItemCommandValidator : AbstractValidator<UpdateShoppingItemCommand>
    {
        public UpdateShoppingItemCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Item Id cannot be empty.");
        }
    }
}
