using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.AddMadeRecipe
{
    public class AddUserMadeRecipeCommandValidator : AbstractValidator<AddUserMadeRecipeCommand>
    {
        public AddUserMadeRecipeCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized");
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Please supply a recipe id.");
        }
    }
}
