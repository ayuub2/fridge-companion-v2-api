using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class ShoppingListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<ShoppingItemDto> Items { get; set; }
        public List<ShoppingListRecipeDto> Recipes { get; set; }
    }
}
