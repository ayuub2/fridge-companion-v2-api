using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Ingredients.Queries.GetIngredientsByName
{
    public class GetIngredientByNameQueryValidator : AbstractValidator<GetIngredientByNameQuery>
    {
        public GetIngredientByNameQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized.");
            RuleFor(x => x.IngredientNames).NotEmpty().WithMessage("At least one ingredient name has to be supplied.");
        }
    }
}
