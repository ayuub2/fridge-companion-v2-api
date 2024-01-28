using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class IngredientDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Carb { get; set; }
        public decimal Sugar { get; set; }
        public decimal Fibre { get; set; }
        public int Standard { get; set; }
        public int Calories { get; set; }
        public int AverageExpiryDays { get; set; }
        public bool IsGlutenFree { get; set; }
        public bool IsNutFree { get; set; }
        public string ImageUrl { get; set; }
        public IngredientLocationDto Location { get; set; }

        public List<IngredientDietDto> DietTypes { get; set; }
        public List<IngredientTypeDto> GroupTypes { get; set; }
        public List<IngredientMeasurementTypeDto> MeasurementTypes { get; set; }
    }
}
