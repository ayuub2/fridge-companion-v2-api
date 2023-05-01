using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        public DbSet<Domain.Entities.User> Users { get; set; }
        public DbSet<UserDiets> UserDiets { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<SuggestionRecipe> SuggestionsRecipes { get; set; }
        DbContext Instance { get; }

        IQueryable<FridgeItem> FreshFridgeItems(string userId);
        IQueryable<Recipe> GetRecipesWithDetails();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        DbSet<ShoppingList> ShoppingList { get; set; }
        DbSet<ShoppingListItem> ShoppingListItem { get; set; }
    }
}
