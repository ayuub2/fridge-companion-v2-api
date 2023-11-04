namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class StatDto
    {
        public StatItem Recipes { get; set; }
        public StatItem ExpiredIngredients { get; set; }
        public StatItem MoneySaved { get; set; }
        public StatItem FavouriteRecipe { get; set; }
        public StatItem MostUsedIngredient { get; set; }
        public StatItem NumberOfIngredientsAdded { get; set; }
    }

    public class StatItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string From { get; set; }
        public bool IsUp { get; set; }
    }
}
