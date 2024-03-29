﻿using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.User.Commands.DeleteFavouriteRecipe
{
    public class DeleteFavouriteRecipeCommandValidator : AbstractValidator<DeleteFavouriteRecipeCommand>
    {
        public DeleteFavouriteRecipeCommandValidator()
        {
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Please supply a recipe id.");
        }
    }
}
