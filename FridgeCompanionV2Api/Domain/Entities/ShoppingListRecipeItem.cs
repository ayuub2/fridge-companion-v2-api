using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class ShoppingListRecipeItem
    {
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }
        public int RecipeId { get; set; }
        public Guid ShoppingListId { get; set; }
        public virtual ShoppingList ShoppingList { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
