using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Fridge.Commands.UpdateFridgeItem
{
    public class UpdateFridgeItemCommandValidator : AbstractValidator<UpdateFridgeItemCommand>
    {
        public UpdateFridgeItemCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("You must provide the Id of the fridge item.");

        }
    }
}
