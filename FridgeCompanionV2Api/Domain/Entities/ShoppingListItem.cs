﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class ShoppingListItem
    {
        public Guid Id { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDeleted { get; set; }
        public decimal Amount { get; set; }
        public Guid ShoppingListId { get; set; }
        public int MeasurementId { get; set; }
        public int IngredientId { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual ShoppingList ShoppingList { get; set; }
        public virtual MeasurementType Measurement { get; set; }
    }
}
