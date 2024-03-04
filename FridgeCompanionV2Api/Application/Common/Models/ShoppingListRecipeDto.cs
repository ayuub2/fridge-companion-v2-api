using System;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class ShoppingListRecipeDto
    {
        public Guid Id { get; set; }
        public int RecipeId { get; set; }
        public string Name { get; set; }
    }
}
