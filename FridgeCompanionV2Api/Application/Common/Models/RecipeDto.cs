using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class RecipeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public int Servings { get; set; }
        public int ReadyInMinutes { get; set; }
        public string Summary { get; set; }
        public string Credit { get; set; }
        public bool IsDeleted { get; set; }
        public List<RecipeIngredientDto> Ingredients { get; set; }
        public List<RecipeDishDto> DishTypes { get; set; }
        public List<RecipeCuisineDto> CuisineTypes { get; set; }
        public List<RecipeStepDto> RecipeSteps { get; set; }
        public int NumberOfIngredients => Ingredients.Count();
        public int NumberOfUsedIngredients { get; set; }
    }
}
