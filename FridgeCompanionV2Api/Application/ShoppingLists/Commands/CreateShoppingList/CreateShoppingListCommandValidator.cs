using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;


namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList
{
    public class CreateShoppingListCommandValidator : AbstractValidator<CreateShoppingListCommand>
    {
        public CreateShoppingListCommandValidator()
        {
        }
    }
}
