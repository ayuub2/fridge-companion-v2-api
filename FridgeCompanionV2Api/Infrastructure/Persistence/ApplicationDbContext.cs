using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FridgeCompanionV2Api.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<TodoItem> TodoItems { get; set; }

        public DbSet<TodoList> TodoLists { get; set; }

        public DbSet<ShoppingList> ShoppingLists { get; set; }
        public DbSet<ShoppingListItem> ShoppingListItems { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<CuisineType> CuisineTypes { get; set; }
        public DbSet<DietType> DietTypes { get; set; }
        public DbSet<DishType> DishTypes { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<IngredientGroupType> IngredientGroupTypes { get; set; }
        public DbSet<IngredientLocation> IngredientLocations { get; set; }
        public DbSet<IngredientMeasurement> ingredientMeasurements { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
