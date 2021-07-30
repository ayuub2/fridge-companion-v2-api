using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Domain.Entities
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Servings { get; set; }
        public int ReadyInMinutes { get; set; }
        public string Summary { get; set; }
        public string Credit { get; set; }
        public bool IsDeleted { get; set; }
        public string AddedBy { get; set; }
        public List<IngredientMeasurement> Ingredients { get; set; }
        public List<DishType> DishTypes { get; set; }
        public List<CuisineType> CuisineTypes { get; set; }
        public List<RecipeStep> RecipeSteps { get; set; }
    }
}
