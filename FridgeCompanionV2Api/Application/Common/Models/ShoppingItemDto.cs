using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class ShoppingItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsChecked { get; set; }
        public bool IsDeleted { get; set; }
        public int MeasurementId { get; set; }
        public int IngredientId { get; set; }
        public string Measurement { get; set; }
        public decimal Amount { get; set; }
    }
}
