using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class FridgeItemDto
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public DateTime Expiration { get; set; }
        public bool IsDeleted { get; set; }
        public IngredientDto Ingredient { get; set; }
        public MeasurementTypeDto Measurement { get; set; }
        public IngredientLocationDto IngredientLocation { get; set; }
    }
}
