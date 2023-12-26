using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingItems.Commands.DeleteAllShoppingItems
{
    public class DeleteAllShoppingItemsCommandValidator : AbstractValidator<DeleteAllShoppingItemsCommand>
    {
        public DeleteAllShoppingItemsCommandValidator()
        {
        }
    }
}
