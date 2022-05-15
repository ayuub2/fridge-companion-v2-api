using System;
using System.Collections.Generic;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class ShoppingList
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public virtual IList<ShoppingListItem> Items { get; private set; } = new List<ShoppingListItem>();
    }
}
