using FluentValidation;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.ClearShoppingList
{
    public class ClearShoppingListCommandValidator : AbstractValidator<ClearShoppingListCommand>
    {
        public ClearShoppingListCommandValidator()
        {
        }
    }
}
