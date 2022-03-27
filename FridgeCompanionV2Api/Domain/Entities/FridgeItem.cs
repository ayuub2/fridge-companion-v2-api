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
        public decimal Amount { get; set; }
        public DateTime Expiration { get; set; }
        public int MeasurementId { get; set; }
        public int IngredientId { get; set; }
        public int IngredientLocationId { get; set; }
        public string UserId { get; set; }
        public bool IsDeleted { get; set; }
        public virtual Ingredient Ingredient { get; set; }

        public virtual MeasurementType Measurement { get; set; }
        public virtual IngredientLocation IngredientLocation { get; set; }
    }
}
