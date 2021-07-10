using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.CreateShoppingList
{
    public class CreateShoppingListCommand : IRequest
    {
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
