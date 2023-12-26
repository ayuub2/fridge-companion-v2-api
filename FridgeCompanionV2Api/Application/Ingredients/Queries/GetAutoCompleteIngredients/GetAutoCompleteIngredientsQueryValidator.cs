using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetAutoCompleteIngredients
{
    public class GetAutoCompleteIngredientsQueryValidator : AbstractValidator<GetAutoCompleteIngredientsQuery>
    {
        public GetAutoCompleteIngredientsQueryValidator()
        {
            RuleFor(x => x.Query).NotEmpty().WithMessage("Query cannot be empty.");
        }
    }
}
