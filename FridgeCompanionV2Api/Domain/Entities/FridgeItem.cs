using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class FridgeItem
    {
        public Guid Id { get; set; }
        public int Amount { get; set; }
        public DateTime Expiration { get; set; }
        public Ingredient Ingredient { get; set; }
        public virtual MeasurementType Measurement { get; set; }
        public virtual IngredientLocation Location { get; set; }
    }
}
