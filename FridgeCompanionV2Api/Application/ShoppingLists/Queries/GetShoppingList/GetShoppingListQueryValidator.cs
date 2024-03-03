using FluentValidation;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Queries.GetShoppingList
{
    public class GetShoppingListQueryValidator : AbstractValidator<GetShoppingListQuery>
    {
        public GetShoppingListQueryValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User is not authorised.");
        }
    }
}
