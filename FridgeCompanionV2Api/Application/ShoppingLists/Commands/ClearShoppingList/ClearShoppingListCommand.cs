using Amazon.Runtime.Internal;
using MediatR;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Commands.ClearShoppingList
{
    public class ClearShoppingListCommand : IRequest<Unit>
    {
        public string UserId { get; set; }
    }
}
