namespace FridgeCompanionV2Api.Application.Common.Models
{
    public class IngredientMeasurementDto
    {
        public int Id { get; set; }
        public IngredientDto Ingredient { get; set; }
        public MeasurementTypeDto Measurement { get; set; }
        public decimal AverageGrams { get; set; }
    }
}