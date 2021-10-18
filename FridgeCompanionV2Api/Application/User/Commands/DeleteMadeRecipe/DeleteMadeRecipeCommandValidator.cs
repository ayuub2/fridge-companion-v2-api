using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.DeleteMadeRecipe
{
    public class DeleteMadeRecipeCommandValidator : AbstractValidator<DeleteMadeRecipeCommand>
    {
        public DeleteMadeRecipeCommandValidator()
        {
            RuleFor(x => x.UserId)
                   .NotEmpty().WithMessage("User is not authorized");
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Please supply a recipe id.");
        }
    }
}
