using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetRecipeById
{
    public class GetRecipeByIdQueryValidator : AbstractValidator<GetRecipeByIdQuery>
    {
        public GetRecipeByIdQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.RecipeId)
                .NotEmpty().WithMessage("Recipe id not supplied.");
        }
    }
}
