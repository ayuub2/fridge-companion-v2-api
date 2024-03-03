using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;

namespace FridgeCompanionV2Api.Application.ShoppingRecipes.Commands.AddShoppingRecipe
{
    public class AddShoppingRecipeCommand : IRequest<ShoppingListRecipeDto>
    {
        public string UserId { get; set; }
        public int RecipeId { get; set; }
        public int ServingSize { get; set; }
    }
}
