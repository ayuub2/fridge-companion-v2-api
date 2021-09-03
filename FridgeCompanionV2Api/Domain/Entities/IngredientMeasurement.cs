using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class IngredientMeasurement
    {
        public int Id { get; set; }
        public double AverageGrams { get; set; }
        public virtual Ingredient Ingredient { get; set; }
        public virtual MeasurementType Measurement { get; set; }
    }
}
