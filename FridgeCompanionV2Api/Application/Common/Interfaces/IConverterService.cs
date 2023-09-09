using FridgeCompanionV2Api.Application.Common.Models;

namespace FridgeCompanionV2Api.Application.Common.Interfaces
{
    public interface IConverterService
    {
        decimal ConvertIngredientAmountToGrams(RecipeIngredientDto ingredient);
    }
}
