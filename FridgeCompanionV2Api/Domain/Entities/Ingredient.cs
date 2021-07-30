using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class Ingredient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Protein { get; set; }
        public decimal Fat { get; set; }
        public decimal Carb { get; set; }
        public decimal Sugar { get; set; }
        public int Standard { get; set; }
        public int Calories { get; set; }
        public int AverageExpiryDays { get; set; }
        public bool IsGluten { get; set; }
        public bool IsDeleted { get; set; }

        public  IngredientLocation Location { get; set; }

        public List<DietType> DietTypes { get; set; }
        public List<IngredientGroupType> GroupTypes { get; set; }
    }
}
