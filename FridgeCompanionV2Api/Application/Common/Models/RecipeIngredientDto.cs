namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class RecipeIngredientDto
    {
        public int Id { get; set; }
        public IngredientDto Ingredient { get; set; }
        public MeasurementTypeDto Measurement { get; set; }
        public int Amount { get; set; }
    }
}