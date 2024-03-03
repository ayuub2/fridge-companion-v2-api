using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;

namespace FridgeCompanionV2Api.Application.ShoppingLists.Queries.GetShoppingList
{
    public class GetShoppingListQuery : IRequest<ShoppingListDto>
    {
        public string UserId { get; set; }
    }
}
