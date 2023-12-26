using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.AddFavouriteRecipe
{
    public class AddFavouriteRecipeCommandValidator : AbstractValidator<AddFavouriteRecipeCommand>
    {
        public AddFavouriteRecipeCommandValidator()
        {
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Please supply a recipe id.");
        }
    }
}
