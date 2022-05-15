using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace FridgeCompanionV2Api.Application.ShoppingItems.Queries.GetShoppingItems
{
    public class GetShoppingItemsCommandValidator : AbstractValidator<GetShoppingItemsCommand>
    {
        public GetShoppingItemsCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("User is not authorized");
        }
    }
}
