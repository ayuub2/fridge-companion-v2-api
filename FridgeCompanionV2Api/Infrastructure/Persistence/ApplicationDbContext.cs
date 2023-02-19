using FridgeCompanionV2Api.Application.Common.Interfaces;
using FridgeCompanionV2Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
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
        public DbSet<RecipeIngredient> RecipeIngredients { get; set; }
        public DbSet<MeasurementType> MeasurementTypes { get; set; }
        public DbSet<RecipeStep> RecipeSteps { get; set; }
        public DbSet<RecipeDish> RecipeDishes { get; set; }
        public DbSet<RecipeCuisine> RecipeCuisines { get; set; }
        public DbSet<IngredientDiet> IngredientDiets { get; set; }
        public DbSet<IngredientType> IngredientTypes { get; set; }
        public DbSet<FridgeItem> FridgeItems { get; set; }
        public DbSet<IngredientMeasurement> IngredientMeasurements { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        public DbSet<SuggestionRecipe> SuggestionsRecipes { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<UserDiets> UserDiets { get; set; }

        public DbContext Instance => this;


        public IQueryable<Recipe> GetRecipesWithDetails() 
        {
            return Recipes
                .Include(x => x.Ingredients)
                    .ThenInclude(x => x.Ingredient)
                        .ThenInclude(x => x.Location)
                .Include(x => x.Ingredients).ThenInclude(x => x.Measurement)
                .Include(x => x.Ingredients).ThenInclude(i => i.Ingredient).ThenInclude(id => id.GroupTypes).ThenInclude(idt => idt.IngredientGroupType)
                .Include(x => x.Ingredients).ThenInclude(i => i.Ingredient).ThenInclude(id => id.DietTypes).ThenInclude(idt => idt.Diet)
                .Include(x => x.DishTypes)
                    .ThenInclude(x => x.Dish)
                .Include(x => x.RecipeSteps)
                .Include(x => x.CuisineTypes)
                    .ThenInclude(x => x.Cuisine).AsNoTracking()
                .Where(x => !x.IsDeleted);
        }
        public IQueryable<FridgeItem> FreshFridgeItems(string userId)
        {
            return FridgeItems.Include(x => x.Ingredient).Include(x=> x.Measurement).Include(x => x.IngredientLocation).Where(x => x.UserId == userId && !x.IsDeleted && DateTime.Now < x.Expiration);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(builder);
        }
    }
}
