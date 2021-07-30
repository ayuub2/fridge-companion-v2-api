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
        public Ingredient Ingredient { get; set; }
        public MeasurementType Measurement { get; set; }
        public decimal AverageGrams { get; set; }
    }
}
