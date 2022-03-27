using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeByServingSize
{
    public class GetRecipeByServingSizeQueryValidator : AbstractValidator<GetRecipeByServingSizeQuery>
    {
        public GetRecipeByServingSizeQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Recipe id not supplied.");
            RuleFor(x => x.ServingSize)
                .NotEmpty().WithMessage("Serving size is not supplied.");

        }
    }
}
