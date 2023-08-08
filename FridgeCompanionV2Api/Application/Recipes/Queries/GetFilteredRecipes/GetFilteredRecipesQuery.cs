using FridgeCompanionV2Api.Application.Common.Models;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Recipes.Queries.GetFilteredRecipes
{
    public class GetFilteredRecipesQuery : IRequest<List<RecipeDto>>
    {
        public string UserId { get; set; }
        public List<int> RecipesToExclude { get; set; } = new List<int>();
        public List<int> Diets { get; set; } = new List<int>();
        public List<int> DishTypes { get; set; } = new List<int>();
        public List<int> CuisineTypes { get; set; } = new List<int>();
        public List<int> Ingredients { get; set; } = new List<int>();
        public string RecipeName { get; set; }
        public NutritionFilter Protein { get; set; }
        public NutritionFilter Calories { get; set; }
        public NutritionFilter Sugar { get; set; }
        public NutritionFilter Fat { get; set; }
        public NutritionFilter Carbs { get; set; }
        public bool UseUserIngredients { get; set; }
    }

    public class NutritionFilter 
    {
        public string Operator { get; set; }
        public int Amount { get; set; }
    }
}
