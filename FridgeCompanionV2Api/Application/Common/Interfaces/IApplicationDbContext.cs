using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<TodoList> TodoLists { get; set; }

        DbSet<TodoItem> TodoItems { get; set; }
        DbSet<Recipe> Recipes { get; set; }
        public DbSet<CuisineType> CuisineTypes { get; set; }
        public DbSet<DietType> DietTypes { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientGroupType> IngredientGroupTypes { get; set; }
        public DbSet<IngredientLocation> IngredientLocations { get; set; }
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeDish> RecipeDishes { get; set; }
        public DbSet<RecipeCuisine> RecipeCuisines { get; set; }
        public DbSet<IngredientDiet> IngredientDiets { get; set; }
        public DbSet<IngredientType> IngredientTypes { get; set; }
        public DbSet<FridgeItem> FridgeItems { get; set; }
        public DbSet<IngredientMeasurement> IngredientMeasurements { get; set; }
        DbContext Instance { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<ShoppingList> ShoppingLists { get; set; }
        DbSet<ShoppingListItem> ShoppingListItems { get; set; }
    }
}
